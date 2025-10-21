public interface IParryable
{
    //EX: knockback, takes damage, etc. 
    void Parry(string charState);
    bool GetParryableNowState();
}