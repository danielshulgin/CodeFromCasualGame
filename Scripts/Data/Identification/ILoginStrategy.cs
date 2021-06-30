public interface ILoginStrategy
{
    void Execute(PlayerData playerData);

    void UnSubscribe();
}
