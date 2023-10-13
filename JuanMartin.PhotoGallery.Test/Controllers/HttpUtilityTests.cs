using JuanMartin.PhotoGallery.Controllers;
using JuanMartin.Kernel.Extesions;

namespace JuanMartin.PhotoGallery.Test.Controllers
{
    public class HttpUtilityTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShoulgGetPreviousAndNextItemsFromAList()
        {
            string actualList = "4,1,3,1,2";

            int baseId = 4;

            var actualPosition = HttpUtility.ImageRelativePosition.Previous;
            int expectedId = -1;
            int actualId = HttpUtility.GetImageId(baseId, actualList, HttpUtility.ImageRelativePosition.Previous);
            Assert.That(actualId, Is.EqualTo(expectedId), $"The {EnumExtensions.GetDescription(actualPosition)} element of {actualId} is not {expectedId}.");
        }
    }
}
