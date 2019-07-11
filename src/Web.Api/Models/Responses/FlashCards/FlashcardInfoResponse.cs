using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ulearn.Web.Api.Models.Responses.FlashCards
{
	[DataContract]
	public class FlashcardInfoResponse
	{
		[DataMember]
		public List<FlashcardsUnitInfo> FlashcardsInfo = new List<FlashcardsUnitInfo>();

		public void Add(FlashcardsUnitInfo flashcardsUnitInfo)
		{
			FlashcardsInfo.Add(flashcardsUnitInfo);
		}
	}
}