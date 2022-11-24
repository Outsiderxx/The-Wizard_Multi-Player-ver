public class ChooseCharacterDialog : Dialog
{
    public int chooseCharaterID { get; private set; }

    public void ChooseCharater(int characterID)
    {
        this.chooseCharaterID = characterID;
        this.Confirm();
        this.Close();
    }
}
