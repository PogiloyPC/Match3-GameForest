using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Grid : Node2D
{
    [Export] private SpawnerGameBoardContent _spawner;

    [Export] private TimerSwap _timer;

    [Export] private PackedScene _prefabExplosion;

    private ElementGameBoard[,] _elementsBoard;

    private ElementGameBoard _selectedElement;

    private Action<IReward> _takeReward;

    Queue<BonusElement> _bonusElements = new Queue<BonusElement>();

    private Vector2I _sizeBoard;

    private int _sizePixel;
    private const int _minMatchCount = 3;
    private const int _maxDistanceSwap = 1;
    private const int _countMatchForSpawnLineBonus = 4;
    private const int _countMatchForSpawnBombBonus = 5;

    public bool IsTouchable { get; private set; } = true;

    [Export] private string _cellPath;

    #region BaseSystem
    public async void LoadGameBoard(Action<IReward> takeReward, ISeterSizeGameBoard seter, int sizePixel)
    {
        _takeReward = takeReward;

        _sizeBoard = seter.GetSizeGameBoard();

        _sizePixel = sizePixel;

        _elementsBoard = new ElementGameBoard[_sizeBoard.X, _sizeBoard.Y];

        await Task.Delay(200);

        for (int x = 0; x < _sizeBoard.X; x++)
        {
            for (int y = 0; y < _sizeBoard.Y; y++)
            {
                PackedScene packed = GD.Load(_cellPath) as PackedScene;

                if (packed.Instantiate() is TileBoard cellBoard)
                {
                    Vector2I position = new Vector2I(y * sizePixel, x * sizePixel);
                    cellBoard.SetPositionCell(position);
                    AddChild(cellBoard);

                    ElementGameBoard element = _spawner.SpawnGameContent();

                    AddChild(element);
                    element.LaunchGameContent(position);
                    element.PlaySpawnAnimation();

                    _elementsBoard[x, y] = element;
                }
            }
        }

        await Task.Delay(200);

        FindAllElementsMatch();
    }

    private async void FindAllElementsMatch()
    {
        IsTouchable = false;

        await Task.Delay(300);

        for (int x = 0; x < _elementsBoard.GetLength(0); x++)
        {
            for (int y = 0; y < _elementsBoard.GetLength(1); y++)
            {
                if (_elementsBoard[y, x].GetTypeContent() != TypeGameBoardContent.emty)
                {
                    MatchElements matchElement = new MatchElements(_elementsBoard[y, x]);

                    FindHorizontalMatch(matchElement);
                    FindVerticalMatch(matchElement);

                    if (matchElement.Count >= _minMatchCount)
                    {
                        await CheckSpawnBonusInMatch(matchElement);
                    }
                }
            }
        }

        if (CheckEmptyElement())
            FindFloatingElement();
        else
            IsTouchable = true;
    }

    private void FindHorizontalMatch(MatchElements matchElements)
    {
        List<ElementGameBoard> elementsHorizontal = new List<ElementGameBoard>();

        ElementGameBoard element = matchElements.GetStartElement();

        int hight = element.GetCordinatInGrid().Y;
        int width = element.GetCordinatInGrid().X;

        for (int column = width + 1; column < _sizeBoard.X; column++)
        {
            if (element.GetTypeContent() == _elementsBoard[hight, column].GetTypeContent())
                elementsHorizontal.Add(_elementsBoard[hight, column]);
            else
                break;
        }

        for (int column = width - 1; column >= 0; column--)
        {
            if (element.GetTypeContent() == _elementsBoard[hight, column].GetTypeContent())
                elementsHorizontal.Add(_elementsBoard[hight, column]);
            else
                break;
        }

        if (elementsHorizontal.Count >= _minMatchCount - 1)
        {
            for (int i = 0; i < elementsHorizontal.Count; i++)
            {
                matchElements.AddMatchElement(elementsHorizontal[i]);
            }

            matchElements.SetHorizontalMathc(elementsHorizontal);
        }
    }

    private void FindVerticalMatch(MatchElements matchElements)
    {
        List<ElementGameBoard> elementsVertical = new List<ElementGameBoard>();

        ElementGameBoard element = matchElements.GetStartElement();

        int hight = element.GetCordinatInGrid().Y;
        int width = element.GetCordinatInGrid().X;

        for (int row = hight + 1; row < _sizeBoard.Y; row++)
        {
            if (element.GetTypeContent() == _elementsBoard[row, width].GetTypeContent())
                elementsVertical.Add(_elementsBoard[row, width]);
            else
                break;
        }

        for (int row = hight - 1; row >= 0; row--)
        {
            if (element.GetTypeContent() == _elementsBoard[row, width].GetTypeContent())
                elementsVertical.Add(_elementsBoard[row, width]);
            else
                break;
        }

        if (elementsVertical.Count >= _minMatchCount - 1)
        {
            for (int i = 0; i < elementsVertical.Count; i++)
            {
                matchElements.AddMatchElement(elementsVertical[i]);
            }

            matchElements.SetVerticalMathc(elementsVertical);
        }
    }

    private bool CheckEmptyElement()
    {
        for (int x = _sizeBoard.X - 1; x >= 0; x--)
            for (int y = _sizeBoard.Y - 1; y >= 0; y--)
                if (_elementsBoard[y, x].GetTypeContent() == TypeGameBoardContent.emty)
                    return true;

        return false;
    }

    private async void FindFloatingElement()
    {
        for (int x = _sizeBoard.X - 1; x >= 0; x--)
        {
            List<Task> putDown = new List<Task>();

            for (int y = _sizeBoard.Y - 1; y >= 0; y--)
            {
                if (_elementsBoard[y, x].GetTypeContent() != TypeGameBoardContent.emty)
                    putDown.Add(PutDownElements(_elementsBoard[y, x]));
            }

            await Task.WhenAll(putDown);
        }

        DelateEmpteElements();
    }

    private async Task PutDownElements(ElementGameBoard element)
    {
        int hight = element.GetCordinatInGrid().Y;
        int width = element.GetCordinatInGrid().X;

        ElementGameBoard emptyElement = null;

        for (int row = hight + 1; row < _sizeBoard.X; row++)
        {
            if (_elementsBoard[_sizeBoard.X - 1, width].GetTypeContent() == TypeGameBoardContent.emty)
            {
                emptyElement = _elementsBoard[_sizeBoard.X - 1, width];

                break;
            }
            else if (_elementsBoard[row, width].GetTypeContent() != TypeGameBoardContent.emty)
            {
                emptyElement = _elementsBoard[row - 1, width];

                break;
            }
        }

        if (emptyElement != null)
        {
            Vector2I cordUp = element.GetCordinatInGrid();
            Vector2I cordDown = emptyElement.GetCordinatInGrid();

            _elementsBoard[cordDown.Y, cordDown.X] = element;
            _elementsBoard[cordUp.Y, cordUp.X] = emptyElement;

            element.SetCordinatInGrid(cordDown);
            emptyElement.SetCordinatInGrid(cordUp);
            element.Move(cordDown * _sizePixel);
            emptyElement.Move(cordUp * _sizePixel);

            await Task.Delay(100);
            await FindElementMatch(element);
        }
    }

    private void DelateEmpteElements()
    {
        for (int x = _sizeBoard.X - 1; x >= 0; x--)
        {
            for (int y = _sizeBoard.Y - 1; y >= 0; y--)
            {
                if (_elementsBoard[y, x].GetTypeContent() == TypeGameBoardContent.emty)
                {
                    ElementGameBoard emptyElement = _elementsBoard[y, x];                                          
                    ElementGameBoard newElement = _spawner.SpawnGameContent();
                    AddChild(newElement);

                    Vector2I launchPosition = emptyElement.GetCordinatInGrid() * _sizePixel;
                    Vector2I cordPosition = emptyElement.GetCordinatInGrid();

                    if (emptyElement == null)
                    {
                        launchPosition = new Vector2I(y, x) * _sizePixel; 
                        cordPosition = new Vector2I(y, x);
                    }
                    else
                    {
                        launchPosition = emptyElement.GetCordinatInGrid() * _sizePixel;
                        cordPosition = emptyElement.GetCordinatInGrid();
                        
                        emptyElement.DestroyElement();
                    }

                    newElement.LaunchGameContent(launchPosition);
                    newElement.PlaySpawnAnimation();

                    _elementsBoard[cordPosition.Y, cordPosition.X] = newElement;
                }
            }
        }

        FindAllElementsMatch();
    }

    #endregion

    #region MatchElement
    private async Task<bool> FindElementMatch(ElementGameBoard element)
    {
        MatchElements matchElement = new MatchElements(element);

        FindHorizontalMatch(matchElement);
        FindVerticalMatch(matchElement);

        if (matchElement.Count >= _minMatchCount)
        {
            await CheckSpawnBonusInMatch(matchElement);
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task HandleMatchElements(MatchElements matchElements)
    {
        for (int i = 0; i < matchElements.Count; i++)
        {
            ElementGameBoard matchElement = matchElements.GetAllElement()[i];

            if (matchElement is BonusElement element)
            {
                CheckBonusElement(matchElements.GetAllElement(), element);

                i--;

                continue;
            }

            ReplaceMatchingElement(matchElement);
        }

        matchElements.ShowMatchingElements();

        while (_bonusElements.Count > 0)
        {
            await HandleBonus(_bonusElements.Dequeue());
        }
    }

    private void ReplaceMatchingElement(ElementGameBoard matchElement)
    {
        _takeReward.Invoke(matchElement);

        MakeEmptyElement(matchElement);
    }
    #endregion

    #region Swap
    public void SelectTouchElement(Vector2I cordTouch)
    {
        _selectedElement = _elementsBoard[cordTouch.Y, cordTouch.X];
        _selectedElement.SelectElement();
    }

    private void UnselectCurrentElement()
    {
        _selectedElement.UnselectElement();
        _selectedElement = null;
    }

    public void SwapElement(Vector2I selectElement, Vector2I swapElement)
    {
        float distance = Mathf.Sqrt((selectElement.X - swapElement.X) * (selectElement.X - swapElement.X) +
            (selectElement.Y - swapElement.Y) * (selectElement.Y - swapElement.Y));

        int convertDistance = Mathf.RoundToInt(distance);

        GD.Print(convertDistance);

        if (convertDistance != _maxDistanceSwap)
        {
            UnselectCurrentElement();
            return;
        }

        if (selectElement.X == swapElement.X || selectElement.Y == swapElement.Y)
        {
            ElementGameBoard _firstSwapElement = _elementsBoard[selectElement.Y, selectElement.X];
            ElementGameBoard _secondSwapElement = _elementsBoard[swapElement.Y, swapElement.X];

            bool isSwappable = _firstSwapElement.GetTypeContent() != TypeGameBoardContent.emty &&
                _secondSwapElement.GetTypeContent() != TypeGameBoardContent.emty;

            if (!isSwappable)
            {
                UnselectCurrentElement();
                return;
            }


            _elementsBoard[swapElement.Y, swapElement.X] = _firstSwapElement;
            _elementsBoard[selectElement.Y, selectElement.X] = _secondSwapElement;

            Vector2I newCordinatFirstElement = _secondSwapElement.GetCordinatInGrid();
            Vector2I newCordinatSecondElement = _firstSwapElement.GetCordinatInGrid();

            _firstSwapElement.SetCordinatInGrid(newCordinatFirstElement);
            _secondSwapElement.SetCordinatInGrid(newCordinatSecondElement);
            _firstSwapElement.Move(newCordinatFirstElement * _sizePixel);
            _secondSwapElement.Move(newCordinatSecondElement * _sizePixel);

            _timer.InitializeTimeout(_firstSwapElement, _secondSwapElement);
            _timer.SetTimeTimer(0.2f);
            _timer.TimeOut += CheckSwapMatch;
            _timer.SetPlay();

            _secondSwapElement.SelectElement();

            IsTouchable = false;
        }
        else
        {
            UnselectCurrentElement();
        }
    }

    private async void CheckSwapMatch(ElementGameBoard firstElement, ElementGameBoard secondElement)
    {
        _timer.TimeOut -= CheckSwapMatch;

        UnselectCurrentElement();
        secondElement.UnselectElement();

        bool firstIsMatched = await FindElementMatch(firstElement);
        bool secondIsMatched = false;

        if (firstElement.GetTypeContent() != secondElement.GetTypeContent())
            secondIsMatched = await FindElementMatch(secondElement);

        if (firstIsMatched || secondIsMatched)
        {
            FindFloatingElement();
        }
        else
        {
            _timer.Timeout += ReloadTouchable;
            _timer.WaitTime = 0.2f;
            _timer.Start();

            CancelSwap(firstElement, secondElement);
        }


    }

    public void ReloadTouchable()
    {
        IsTouchable = true;

        _timer.Timeout -= ReloadTouchable;
    }

    private void CancelSwap(ElementGameBoard selectElement, ElementGameBoard swapElement)
    {
        Vector2I firstCord = selectElement.GetCordinatInGrid();
        Vector2I secondCord = swapElement.GetCordinatInGrid();


        _elementsBoard[firstCord.Y, firstCord.X] = swapElement;
        _elementsBoard[secondCord.Y, secondCord.X] = selectElement;

        selectElement.SetCordinatInGrid(secondCord);
        swapElement.SetCordinatInGrid(firstCord);
        selectElement.Move(secondCord * _sizePixel);
        swapElement.Move(firstCord * _sizePixel);
    }
    #endregion

    #region BonusLoade
    private async Task CheckSpawnBonusInMatch(MatchElements match)
    {
        if (match.CountHorizontalMatch >= _countMatchForSpawnBombBonus - 2 && match.CountVerticalMatch >= _countMatchForSpawnBombBonus - 2)
            await LoadBonus(match, TypeBonusElement.bomb);
        else if (match.CountHorizontalMatch >= _countMatchForSpawnBombBonus || match.CountVerticalMatch >= _countMatchForSpawnBombBonus)
            await LoadBonus(match, TypeBonusElement.bomb);
        else if (match.CountHorizontalMatch == _countMatchForSpawnLineBonus)
            await LoadBonus(match, TypeBonusElement.horizontalLine);
        else if (match.CountVerticalMatch == _countMatchForSpawnLineBonus)
            await LoadBonus(match, TypeBonusElement.verticalLine);
        else
            await HandleMatchElements(match);
    }

    private async Task LoadBonus(MatchElements match, TypeBonusElement typeBonus)
    {
        ElementGameBoard startMatchElement = match.GetStartElement();

        BonusElement bonusElement = (BonusElement)_spawner.SpawnBonus(typeBonus);

        Vector2I position = startMatchElement.GetCordinatInGrid() * _sizePixel;

        if (bonusElement != null)
        {
            AddChild(bonusElement);

            bonusElement.LaunchGameContent(position);

            await LoadBonusInGrid(match, bonusElement, startMatchElement);
        }
    }

    private async Task LoadBonusInGrid(MatchElements match, BonusElement bonusElement, ElementGameBoard startElement)
    {
        for (int i = 0; i < match.Count; i++)
        {
            if (match.GetAllElement()[i] is BonusElement bonus)
                continue;

            match.GetAllElement()[i].Move(bonusElement.GetPosition());
        }

        match.GetAllElement().Remove(startElement);

        Vector2I cordinatBonusElement = startElement.GetCordinatInGrid();

        Color colorBonus = startElement.GetColorElement();

        TypeGameBoardContent typeLauchBonus = startElement.GetTypeContent();

        startElement.DestroyElement();

        bonusElement.InitializeBonus(colorBonus, typeLauchBonus);

        _elementsBoard[cordinatBonusElement.Y, cordinatBonusElement.X] = bonusElement;

        await HandleMatchElements(match);
    }
    #endregion

    #region BonusHandle
    private async Task HandleBonus(BonusElement element)
    {
        switch (element.GetTypeBonus())
        {
            case TypeBonusElement.horizontalLine:
            case TypeBonusElement.verticalLine:
                await PlayLineBonus(element);
                break;
            case TypeBonusElement.bomb:
                await PlayBombBonus(element);
                break;
            default:
                break;
        }
    }

    private async Task PlayLineBonus(BonusElement element)
    {
        Vector2I cordinatBonus = element.GetCordinatInGrid();

        ReplaceMatchingElement(element);

        element.DestroyElement();

        var line = new List<ElementGameBoard>();

        if (element.GetTypeBonus() == TypeBonusElement.horizontalLine)
            line = HandlerMatrix<ElementGameBoard>.GetListRow(_elementsBoard, cordinatBonus.Y);
        else
            line = HandlerMatrix<ElementGameBoard>.GetListColumn(_elementsBoard, cordinatBonus.X);

        Destroyer rightDestroyer = LoadDestroyer(cordinatBonus);
        Destroyer leftDestroyer = LoadDestroyer(cordinatBonus);

        for (int i = 0; i < line.Count; i++)
        {
            if (line[i] is BonusElement bonus)
            {
                CheckBonusElement(line, bonus);

                i--;

                continue;
            }

            ReplaceMatchingElement(line[i]);
        }

        HandleDestroyer(rightDestroyer, line, element.GetTypeBonus(), true, 1);
        HandleDestroyer(leftDestroyer, line, element.GetTypeBonus(), false, -1);

        await Task.Delay(100 * line.Count);
    }

    private async Task PlayBombBonus(BonusElement element)
    {
        int numberRow = element.GetCordinatInGrid().Y - 1;
        int numberColumn = element.GetCordinatInGrid().X - 1;

        ReplaceMatchingElement(element);

        SetExplosionParticles(element);

        element.DestroyElement();

        var matrix3x3 = HandlerMatrix<ElementGameBoard>.GetElementMatrix3X3(_elementsBoard, numberRow, numberColumn);

        for (int i = 0; i < matrix3x3.Count; i++)
        {
            if (matrix3x3[i] is BonusElement bonus)
            {
                CheckBonusElement(matrix3x3, bonus);

                i--;

                continue;
            }

            ReplaceMatchingElement(matrix3x3[i]);
        }

        await ClearMatrix(matrix3x3);
    }

    private async void HandleDestroyer(Destroyer destroyer, List<ElementGameBoard> elements, TypeBonusElement type, bool isRight, int direction)
    {
        int startElement = 0;

        if (type == TypeBonusElement.horizontalLine)
            startElement = Mathf.RoundToInt(destroyer.Position.X) / _sizePixel;
        else
            startElement = Mathf.RoundToInt(destroyer.Position.Y) / _sizePixel;

        for (int i = startElement += direction; i < elements.Count && i >= 0; i += direction)
        {
            if (elements[i] != null)
            {
                destroyer.MoveNextPosition(elements[i].GetCordinatInGrid() * _sizePixel, isRight);

                await Task.Delay(100);

                elements[i].DestroyElement();
            }
        }

        destroyer.Free();
    }

    private void CheckBonusElement(List<ElementGameBoard> elements, BonusElement bonus)
    {
        if (_bonusElements.Count < 1)
        {
            elements.Remove(bonus);

            _bonusElements.Enqueue(bonus);
        }
        else
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (bonus != elements[i])
                {
                    if (bonus != null)
                    {
                        elements.Remove(bonus);

                        _bonusElements.Enqueue(bonus);
                    }
                }
            }
        }
    }

    private async Task ClearMatrix(List<ElementGameBoard> elements)
    {
        await Task.Delay(250);

        for (int i = 0; i < elements.Count; i++)
        {
            elements[i].DestroyElement();
        }
    }
    #endregion

    #region AddiotionalMethods
    private void MakeEmptyElement(ElementGameBoard element)
    {
        Vector2I cordinatNewElement = element.GetCordinatInGrid();
        Vector2I positionNewElement = element.GetCordinatInGrid() * _sizePixel;

        ElementGameBoard emptyElement = _spawner.SpawnEmptyElement();

        if (emptyElement != null)
        {
            AddChild(emptyElement);

            emptyElement.LaunchGameContent(positionNewElement);

            _elementsBoard[cordinatNewElement.Y, cordinatNewElement.X] = emptyElement;
        }
    }

    private void SetExplosionParticles(ElementGameBoard element)
    {
        if (_prefabExplosion.Instantiate() is ExplosionParticles explosion)
        {
            AddChild(explosion);

            explosion.SetColorExplosion(element.GetColorElement());
            explosion.SetPosition(element.GetPosition());
            explosion.Play();
        }
    }

    private Destroyer LoadDestroyer(Vector2I launchPosition)
    {
        Destroyer destroyer = _spawner.SpawnDestroyer();

        launchPosition *= _sizePixel;

        if (destroyer != null)
        {
            AddChild(destroyer);

            destroyer.Position = launchPosition;
            return destroyer;
        }

        return null;
    }
    #endregion
}

