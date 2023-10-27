using JuanMartin.PhotoGallery.Controllers;
using JuanMartin.Kernel.Extesions;
using Microsoft.AspNetCore.Mvc;

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

            // test begginnning of list
            int baseId = 4;

            var actualPosition = HttpUtility.ImageRelativePosition.Previous;
            int expectedId = -1;
            int actualId = HttpUtility.GetImageId(baseId, actualList, actualPosition);
            Assert.That(actualId, Is.EqualTo(expectedId), $"The {EnumExtensions.GetDescription(actualPosition)} element of {actualId} is not {expectedId}.");
            actualPosition = HttpUtility.ImageRelativePosition.Next;
            expectedId = 1;
            actualId = HttpUtility.GetImageId(baseId, actualList, actualPosition);
            Assert.That(actualId, Is.EqualTo(expectedId), $"The {EnumExtensions.GetDescription(actualPosition)} element of {actualId} is not {expectedId}.");

            // test end of list
            baseId = 2;

            actualPosition = HttpUtility.ImageRelativePosition.Next;
            expectedId = -1;
            actualId = HttpUtility.GetImageId(baseId, actualList, actualPosition);
            Assert.That(actualId, Is.EqualTo(expectedId), $"The {EnumExtensions.GetDescription(actualPosition)} element of {actualId} is not {expectedId}.");
            actualPosition = HttpUtility.ImageRelativePosition.Previous;
            expectedId = 1;
            actualId = HttpUtility.GetImageId(baseId, actualList, actualPosition);
            Assert.That(actualId, Is.EqualTo(expectedId), $"The {EnumExtensions.GetDescription(actualPosition)} element of {actualId} is not {expectedId}.");

            // test end of list
            baseId = 2;

            actualPosition = HttpUtility.ImageRelativePosition.Next;
            expectedId = -1;
            actualId = HttpUtility.GetImageId(baseId, actualList, actualPosition);
            Assert.That(actualId, Is.EqualTo(expectedId), $"The {EnumExtensions.GetDescription(actualPosition)} element of {actualId} is not {expectedId}.");
            actualPosition = HttpUtility.ImageRelativePosition.Previous;
            expectedId = 1;
            actualId = HttpUtility.GetImageId(baseId, actualList, actualPosition);
            Assert.That(actualId, Is.EqualTo(expectedId), $"The {EnumExtensions.GetDescription(actualPosition)} element of {actualId} is not {expectedId}.");

            // test elements with multiple instances
            baseId = 1;

            actualPosition = HttpUtility.ImageRelativePosition.Next;
            expectedId = 3;
            actualId = HttpUtility.GetImageId(baseId, actualList, actualPosition);
            Assert.That(actualId, Is.EqualTo(expectedId), $"The {EnumExtensions.GetDescription(actualPosition)} element of {actualId} is not {expectedId}.");
            actualPosition = HttpUtility.ImageRelativePosition.Previous;
            expectedId = 4;
            actualId = HttpUtility.GetImageId(baseId, actualList, actualPosition);
            Assert.That(actualId, Is.EqualTo(expectedId), $"The {EnumExtensions.GetDescription(actualPosition)} element of {actualId} is not {expectedId}.");

            int actualInstance = 2;

            actualPosition = HttpUtility.ImageRelativePosition.Next;
            expectedId = 2;
            actualId = HttpUtility.GetImageId(baseId, actualList, actualPosition,actualInstance);
            Assert.That(actualId, Is.EqualTo(expectedId), $"The {EnumExtensions.GetDescription(actualPosition)} element of {actualId} is not {expectedId}.");
            actualPosition = HttpUtility.ImageRelativePosition.Previous;
            expectedId = 3;
            actualId = HttpUtility.GetImageId(baseId, actualList, actualPosition,actualInstance);
            Assert.That(actualId, Is.EqualTo(expectedId), $"The {EnumExtensions.GetDescription(actualPosition)} element of {actualId} is not {expectedId}.");
        }
    }
}
