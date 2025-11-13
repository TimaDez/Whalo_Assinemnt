using System.Collections.Generic;
using Navigation;
using Popups;

namespace Whalo.Controllers
{
    public class LoadingPopupsController : LoadingControllerBase
    {
        #region Methods

        protected override void CreateUrlsList()
        {
            _eventsPopups = new List<string>
            {
                NetworkNavigation.COINS_EVENT_POPUP_LINK,
                NetworkNavigation.KEYS_EVENT_POPUP_LINK
            };
        }

        #endregion
    }
}