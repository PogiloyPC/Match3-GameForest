using Godot;
using System.Threading.Tasks;
using System.Collections.Generic;

public class MatchElements
{
    private List<ElementGameBoard> _matchElements = new List<ElementGameBoard>();

    private List<ElementGameBoard> _horizontalMatch = new List<ElementGameBoard>();
    private List<ElementGameBoard> _verticalMatch = new List<ElementGameBoard>();

    public int CountVerticalMatch => _verticalMatch.Count + 1;
    public int CountHorizontalMatch => _horizontalMatch.Count + 1;

    public int Count => _matchElements.Count;

    public MatchElements(ElementGameBoard startElement) => _matchElements.Add(startElement);

    public void AddMatchElement(ElementGameBoard element)
    {        
        _matchElements.Add(element);
    }

    public void SetHorizontalMathc(List<ElementGameBoard> horizontalMatch)
    {
        _horizontalMatch = horizontalMatch;
    }

    public void SetVerticalMathc(List<ElementGameBoard> verticalMatch)
    {
        _verticalMatch = verticalMatch;
    }

    public async void ShowMatchingElements()
    {
        //foreach (ElementGameBoard element in _matchElements)           
        //        element.StartDestroyAnimation();

        //await Task.Delay(250);

        foreach (ElementGameBoard element in _matchElements)            
                element.FinishDestroyAnimation();

        await Task.Delay(250);

        foreach (ElementGameBoard element in _matchElements)            
                element.DestroyElement();
    }

    public ElementGameBoard GetStartElement() => _matchElements[0];

    public List<ElementGameBoard> GetAllElement() => _matchElements;
}

