using OxGKit.ActionSystem;

public class NewTplAction : ActionBase
{
    #region Default Constructor
    public NewTplAction()
    {
        this.name = nameof(NewTplAction);
    }
    
    public NewTplAction(int uid) : this()
    {
        this.uid = uid;
    }
    
    public NewTplAction(string name)
    {
        this.name = string.IsNullOrEmpty(name) ? nameof(NewTplAction) : name;
    }
    
    public NewTplAction(string name, int uid) : this(name)
    {
        this.uid = uid;
    }
    #endregion

    /**
     * Declare your params
     */

    /// <summary>
    /// [Factory Mode] You can create a static method to instance an action (depends on you or not)
    /// </summary>
    public static NewTplAction CreateNewTplAction(/* Define params */)
    {
        var action = new NewTplAction();

        /**
         * Assign params...
         */

        return action;
    }

    protected override void OnStart()
    {
        // Set -1 is end by condition or duration time (-1 = have to MarkAsDone manually)
        this.SetDuration(-1);

        /**
         * Example:
         * Do Somethings by Condition (You can declare a method to define your program or just do in here)
         * .
         * .
         * .
         * And then if SetDuration(-1), the end must invoke MarkAsDone
         * this.MarkAsDone();
         */
    }
}
