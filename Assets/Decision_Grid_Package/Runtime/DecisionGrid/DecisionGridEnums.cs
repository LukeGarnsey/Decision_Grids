namespace DecisionGrid
{
    public enum BreakType{
        None, 
        Neutral, 
        Positive, 
        Negative, 
        NeutralSetValue, 
        PositiveSetValue, 
        NegativeSetValue
    }
    public enum Shape{
        Square, 
        Diamond, 
        Circle,
        Line,
        Cone
    }
    public enum NavigationCheckFor{
        Neutral, 
        Positive, 
        Negative
    }
    public enum NavigationPersistence{
        VeryLow = 25,
        Low = 50,
        Medium = 100,
        High = 150,
        VeryHigh = 250,
        Extreme = 500
    }
}