public interface IUpgradeReceiver<TUpgrade> where TUpgrade : EntityUpgrade
{
    void ReceiveUpgrade(TUpgrade upgrade);
}