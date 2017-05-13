using System.Collections.Generic;
using UnityEngine;

namespace googleVisionAPI{
	
	public class VisionRestAPI : MonoBehaviour{

		///Start->Request class, to be encoded into a JSON file
		[System.Serializable]
		public class AnnotateImageRequests {
			public List<AnnotateImageRequest> requests;
		}

		[System.Serializable]
		public class AnnotateImageRequest {
			public Image image;
			public List<Feature> features;
			//public ImageContext image_context; //throws error if not given a value, so comment out if not needed
		}

		[System.Serializable]
		public class Image {
			public string content;
			public ImageSource source;
		}

		[System.Serializable]
		public class ImageSource {
			public string gcsImageUri;
			public string imageUri;
		}

		[System.Serializable]
		public class Feature {
			public string type;
			public int maxResults;
		}

		[System.Serializable]
		public class ImageContext {
			public LatLongRect latLongRect;
			public List<string> languageHints;
			public List<CropHintsParams> crop_hints_params;
		}

		[System.Serializable]
		public class CropHintsParams {
			public List<float> aspect_ratios;
		}

		[System.Serializable]
		public class LatLongRect {
			public LatLng minLatLng;
			public LatLng maxLatLng;
		}

		public enum FeatureType {
			TYPE_UNSPECIFIED, //Unspecified feature type.
			FACE_DETECTION, //Run face detection.
			LABEL_DETECTION, //Run landmark detection.
			LANDMARK_DETECTION, //Run logo detection.
			LOGO_DETECTION, //Run label detection.
			TEXT_DETECTION, //Run OCR.
			DOCUMENT_TEXT_DETECTION, //Run dense text document OCR. Takes precedence when both DOCUMENT_TEXT_DETECTION and TEXT_DETECTION are present.
			SAFE_SEARCH_DETECTION, //Run computer vision models to compute image safe-search properties.
			IMAGE_PROPERTIES, //Compute a set of image properties, such as the image's dominant colors.
			CROP_HINTS, //Run crop hints.
			WEB_DETECTION //Run web detection.
		}
		///End<-Request class, to be encoded into a JSON file


		///Start->Returned JSON file converted to classes
		[System.Serializable]
		public class AnnotateImageResponses {
			public List<AnnotateImageResponse> responses;
		}

		//Feature response classes
		[System.Serializable]
		public class AnnotateImageResponse {
			public List<FaceAnnotation> faceAnnotations;
			public List<EntityAnnotation> landmarkAnnotations;
			public List<EntityAnnotation> logoAnnotations;
			public List<EntityAnnotation> labelAnnotations;
			public List<EntityAnnotation> textAnnotations;
			public TextAnnotation fullTextAnnotation;
			public SafeSearchAnnotation safeSearchAnnotation;
			public ImageProperties imagePropertiesAnnotation;
			public CropHintsAnnotation cropHintsAnnotation;
			public WebDetection webDetection;
		}


		//Start->FaceAnnotation
		//Face Detection detects multiple faces within an image along with the associated key facial attributes
		//such as emotional state or wearing headwear. Facial Recognition is not supported.
		[System.Serializable]
		public class FaceAnnotation {
			public BoundingPoly boundingPoly;
			public BoundingPoly fdBoundingPoly;
			public List<Landmark> landmarks;
			public float rollAngle;
			public float panAngle;
			public float tiltAngle;
			public float detectionConfidence;
			public float landmarkingConfidence;
			public string joyLikelihood;
			public string sorrowLikelihood;
			public string angerLikelihood;
			public string surpriseLikelihood;
			public string underExposedLikelihood;
			public string blurredLikelihood;
			public string headwearLikelihood;
		}

		[System.Serializable]
		public class Landmark {
			public string type;
			public Position position;
		}
			
		public enum LandmarkType {
			UNKNOWN_LANDMARK,
			LEFT_EYE,
			RIGHT_EYE,
			LEFT_OF_LEFT_EYEBROW,
			RIGHT_OF_LEFT_EYEBROW,
			LEFT_OF_RIGHT_EYEBROW,
			RIGHT_OF_RIGHT_EYEBROW,
			MIDPOINT_BETWEEN_EYES,
			NOSE_TIP,
			UPPER_LIP,
			LOWER_LIP,
			MOUTH_LEFT,
			MOUTH_RIGHT,
			MOUTH_CENTER,
			NOSE_BOTTOM_RIGHT,
			NOSE_BOTTOM_LEFT,
			NOSE_BOTTOM_CENTER,
			LEFT_EYE_TOP_BOUNDARY,
			LEFT_EYE_RIGHT_CORNER,
			LEFT_EYE_BOTTOM_BOUNDARY,
			LEFT_EYE_LEFT_CORNER,
			RIGHT_EYE_TOP_BOUNDARY,
			RIGHT_EYE_RIGHT_CORNER,
			RIGHT_EYE_BOTTOM_BOUNDARY,
			RIGHT_EYE_LEFT_CORNER,
			LEFT_EYEBROW_UPPER_MIDPOINT,
			RIGHT_EYEBROW_UPPER_MIDPOINT,
			LEFT_EAR_TRAGION,
			RIGHT_EAR_TRAGION,
			LEFT_EYE_PUPIL,
			RIGHT_EYE_PUPIL,
			FOREHEAD_GLABELLA,
			CHIN_GNATHION,
			CHIN_LEFT_GONION,
			CHIN_RIGHT_GONION
		};
		//End<-FaceAnnotation


		//Start->EntityAnnotation
		//LandMarkAnnotations: Landmark Detection detects popular natural and man-made structures within an image.
		//LogoAnnotations: Logo Detection detects popular product logos within an image.
		//LabelAnnotations: Label Detection detects broad sets of categories within an image, which range from modes of transportation to animals.
		//TextAnnotations: Text Detection performs Optical Character Recognition.
		//It detects and extracts text within an image with support for a broad range of languages. It also features automatic language identification.
		[System.Serializable]
		public class EntityAnnotation {
			public string mid;
			public string locale;
			public string description;
			public float score;
			public float confidence;
			public float topicality;
			public BoundingPoly boundingPoly;
			public List<LocationInfo> locations;
			public List<Property> properties;
		}
		//End<-EntityAnnotation


		//Start->FullTextAnnotation
		//Document Text Detection performs Optical Character Recognition. This feature detects dense document text in an image.
		//Detects the location of text in the image
		[System.Serializable]
		public class TextAnnotation {
			//public List<Page> pages; //throws error, unsure how to fix. Information not useful to me yet.
			public string text;
		}
			
		[System.Serializable]
		public class Page {
			public TextProperty property;
			public float width;
			public float height;
			public List<Block> blocks;
		}

		[System.Serializable]
		public class TextProperty {
			public List<DetectedLanguage> detectedLanguages;
			public DetectedBreak detectedBreak;
		}

		[System.Serializable]
		public class DetectedLanguage {
			public string languageCode;
			public float confidence;
		}

		[System.Serializable]
		public class DetectedBreak {
			public BreakType type;
			public bool isPrefix;
		}

		[System.Serializable]
		public class Block {
			public TextProperty property;
			public BoundingPoly boundingBox;
			public List<Paragraph> paragraphs;
			public BlockType blockType;
		}

		[System.Serializable]
		public class Paragraph {
			public TextProperty property;
			public BoundingPoly boundingBox;
			public List<Word> words;
		}

		[System.Serializable]
		public class Word {
			public TextProperty property;
			public BoundingPoly boundingBox;
			public List<Symbol> symbols;
		}

		[System.Serializable]
		public class Symbol {
			public TextProperty property;
			public BoundingPoly boundingBox;
			public string text;
		}
			
		public enum BreakType {
			UNKOWN, //Unknown break label type
			SPACE, //Regular space.
			SURE_SPACE, //Sure space (very wide).
			EOL_SURE_SPACE, //Line-wrapping break.
			HYPHEN, //End-line hyphen that is not present in text
			LINE_BREAK //Line break that ends a paragraph.
		}
			
		public enum BlockType {
			UNKOWN, //Unknown block type.
			TEXT, //Regular text block.
			TABLE, //Table block.
			PICTURE, //Image block.
			RULER, //Horizontal/vertical line box.
			BARCODE //Barcode block.
		}
		//End<-FullTextAnnotation


		//Start->SafeSearchAnnotation
		//Safe Search Detection detects explicit content such as adult content or violent content within an image.
		[System.Serializable]
		public class SafeSearchAnnotation {
			public Likelihood adult; //Represents the adult content likelihood for the image.
			public Likelihood spoof; //The likelihood that an modification was made to the image's canonical version to make it appear funny or offensive.
			public Likelihood medical; //Likelihood that this is a medical image.
			public Likelihood violence; //Violence likelihood.
		}
		//End<-SafeSearchAnnotation


		//Start->ImageProperties
		//The Image Properties feature detects general attributes of the image, such as dominant color.
		[System.Serializable]
		public class ImageProperties {
			public DominantColorsAnnotation dominantColors;
		}

		[System.Serializable]
		public class DominantColorsAnnotation {
			public List<ColorInfo> colors;
		}

		[System.Serializable]
		public class ColorInfo {
			public Color color;
			public float score;
			public float pixelFraction;
		}

		[System.Serializable]
		public class Color {
			public int red;
			public int green;
			public int blue;
			public int alpha;
		}
		//End<-ImageProperties


		//Start->CropHints
		//Crop Hints suggests vertices for a crop region on an image.
		[System.Serializable]
		public class CropHintsAnnotation {
			public List<CropHint> cropHints;
		}

		[System.Serializable]
		public class CropHint {
			public BoundingPoly boundingPoly;
			public float confidence;
			public float importanceFraction;
		}
		//End<-CropHints


		//Start->WebDetection
		//Web Detection detects Web references to an image.
		//Gives URL to images that fully match image, partially match image,
		//web pages with matching image, and visually similar image.
		[System.Serializable]
		public class WebDetection {
			public List<WebEntity> webEntities; //Deduced entities from similar images on the Internet.
			public List<WebImage> fullMatchingImages; //Fully matching images from the Internet. They're definite neardups and most often a copy of the query image with merely a size change.
			public List<WebImage> partialMatchingImages; //Partial matching images from the Internet. Those images are similar enough to share some key-point features. For example an original image will likely have partial matching for its crops.
			public List<WebPage> pagesWithMatchingImages; //Web pages containing the matching images from the Internet.
			public List<WebImage> visuallySimilarImages; //The visually similar image results.
		}

		[System.Serializable]
		public class WebEntity {
			public string entityId;
			public float score;
			public string description;
		}

		[System.Serializable]
		public class WebImage {
			public string url;
			public float score;
		}

		[System.Serializable]
		public class WebPage {
			public string url;
			public float score;
		}
		//End<-WebDetection


		//Start<-classes and enums shared by all Annotations
		[System.Serializable]
		public class BoundingPoly {
			public List<Vertex> vertices;
		}

		[System.Serializable]
		public class LocationInfo {
			public LatLng latLng;
		}

		[System.Serializable]
		public class Property {
			public string name;
			public string value;
			public string uint64Value;
		}

		[System.Serializable]
		public class Position {
			public float x;
			public float y;
			public float z;
		}

		[System.Serializable]
		public class Vertex {
			public float x;
			public float y;
		}
			
		[System.Serializable]
		public class LatLng {
			public float latitude;
			public float longitude;
		}

		public enum Likelihood {
			UNKNOWN, //Unknown likelihood.
			VERY_UNLIKELY, //It is very unlikely that the image belongs to the specified vertical.
			UNLIKELY, //It is unlikely that the image belongs to the specified vertical.
			POSSIBLE, //It is possible that the image belongs to the specified vertical.
			LIKELY, //It is likely that the image belongs to the specified vertical.
			VERY_LIKELY //It is very likely that the image belongs to the specified vertical.
		}
		//End<-Classes shared by all Annotations
		///End<-Returned JSON file converted to classes

	}
}