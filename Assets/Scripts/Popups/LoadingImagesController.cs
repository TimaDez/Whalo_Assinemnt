using System.Collections.Generic;
using Navigation;
using Popups;

namespace Whalo.Controllers
{
    public class LoadingImagesController : LoadingControllerBase
    {
        #region Methods

        protected override void CreateUrlsList()
        {
            _eventsPopups = new List<string>
            {
                NetworkNavigation.COINS_IMAGE_LINK,
                NetworkNavigation.ENERGY_IMAGE_LINK,
                NetworkNavigation.KEY_IMAGE_LINK
            };
        }

        #endregion
    }
}