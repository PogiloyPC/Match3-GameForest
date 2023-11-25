using Godot;

public partial class ViewTotalScore : Label
{
    public void DisplayTotalScore(IGameScore gameScore) => Text = gameScore.GetTotalScore().ToString();
}

