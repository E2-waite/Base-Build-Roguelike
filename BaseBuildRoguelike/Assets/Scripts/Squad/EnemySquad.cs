public class EnemySquad : Squad
{
    void Start()
    {
        Enemies.squads.Add(this);
    }

    public override void Setup(Interaction member1, Interaction member2)
    {
        Enemy enemy1 = member1 as Enemy, enemy2 = member2 as Enemy;
        members.Add(enemy1);
        members.Add(enemy2);
        enemy1.squad = this;
        enemy2.squad = this;
    }

    public override void UpdateMembers()
    {
        for (int i = 0; i < members.Count; i++)
        {
            (members[i] as Enemy).squad = this;
        }
    }


    public override void AddMember(Interaction newMember)
    {
        members.Add(newMember);
        (newMember as Enemy).squad = this;
    }

    public override void Disband()
    {
        Enemies.squads.Remove(this);
        Destroy(gameObject);
    }
}
