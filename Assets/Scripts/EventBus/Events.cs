public interface IEvent { }

public struct TestEvent : IEvent { }

public struct PlayerEvent : IEvent {
    public int health;
    public int mana;
}
public struct EnemyAttack : IEvent
{
    public int damage;
    public string animName;
}

public struct DrankAlcoholEvent : IEvent
{
    public float alcholAmount;
}

#region Quest Events

public struct OnStartQuestEvent : IEvent
{
    public string questID;
}

public struct OnAdvanceQuestEvent : IEvent
{
    public string questID;
}

public struct OnFinishQuestEvent : IEvent
{
    public string questID;
}

public struct OnQuestStateChangeEvent : IEvent
{
    public DrunkSimulator.Quest.Quest quest;
}

#endregion