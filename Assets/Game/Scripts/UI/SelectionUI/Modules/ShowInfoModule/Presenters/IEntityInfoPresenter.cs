using System.Collections.Generic;

namespace Game.Scripts.UI.Modules.Presenters
{
    public interface IEntityInfoPresenter
    {
        void Show(List<Entity> entities);
        void Hide();
    }
}