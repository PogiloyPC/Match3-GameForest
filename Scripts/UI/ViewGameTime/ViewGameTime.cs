using Godot;
public partial class ViewGameTime : Label, IViewGameTime
{
    public void DisplayGameTime(IGameTime gameTime)
    {       
        int sec = (int)gameTime.GetCurrentGameTime();
        double milsec = (gameTime.GetCurrentGameTime() - sec) * 100;
        milsec = Mathf.Round(milsec);

        string secStr = sec.ToString("00");
        string milsecStr = milsec.ToString("00");
        
        Text = string.Format($"{secStr}:{milsecStr}");
    }
}
