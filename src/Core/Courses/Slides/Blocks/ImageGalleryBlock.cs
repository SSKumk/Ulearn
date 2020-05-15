using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Ulearn.Core.Courses.Slides.Blocks.Api;
using Ulearn.Core.Model.Edx.EdxComponents;

namespace Ulearn.Core.Courses.Slides.Blocks
{
	[XmlType("galleryImages")]
	public class ImageGalleryBlock : SlideBlock, IApiSlideBlock
	{
		[XmlElement("image")]
		[IgnoreDataMember]
		public string[] RelativeToUnitDirectoryImagePaths { get; set; }

		[DataMember(Name = "imageUrls")]
		[XmlIgnore]
		public string[] ImageUrls {
			get
			{
				return RelativeToUnitDirectoryImagePaths.Select(p => Path.Combine(BaseUrl, p)).ToArray();
			}
			set { }
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public string BaseUrl;

		public ImageGalleryBlock(string[] relativeToUnitDirectoryImagePaths, string baseUrl)
			: this(relativeToUnitDirectoryImagePaths)
		{
			this.BaseUrl = baseUrl;
		}

		public ImageGalleryBlock(string[] relativeToUnitDirectoryImagePaths)
		{
			RelativeToUnitDirectoryImagePaths = relativeToUnitDirectoryImagePaths;
		}

		public ImageGalleryBlock()
		{
		}

		public override IEnumerable<SlideBlock> BuildUp(SlideBuildingContext context, IImmutableSet<string> filesInProgress)
		{
			BaseUrl ??= context.Slide.Info.DirectoryRelativePath;
			yield return this;
		}

		public override string ToString()
		{
			return $"Gallery with images:\n{string.Join("\n", RelativeToUnitDirectoryImagePaths)}";
		}

		public override Component ToEdxComponent(string displayName, string courseId, Slide slide, int componentIndex, string ulearnBaseUrl, DirectoryInfo coursePackageRoot)
		{
			var urlName = slide.NormalizedGuid + componentIndex;
			return new GalleryComponent(urlName, displayName, urlName, ImageUrls);
		}

		[XmlIgnore]
		[DataMember(Name = "type")]
		public string Type { get; set; } = "imageGallery";
	}
}