using UnityEngine;
using Xareus.Relations.Unity;

//====================================================================================
//
//       ! WARNING : AUTOMATICALLY GENERATED FILE ! 
//    This file risks being overwriten : do not modify manually
//       Modify through Xareus -> Relations -> Open Relation Editor 
//
//====================================================================================

namespace Xareus.Samples.RelationEngineQueries {

	[RelationDescriptorAttribute("", GenerationMode.Automatic)]
	public partial class ComplexRelation : Xareus.Relations.Unity.XURelation
	{


		[ObjectPatternDescriptor("", GenerationMode.Automatic)]
		[ObjectPatternAttribute("SimpleObjectPattern","SampleType","")]
		[ParameterDescriptorAttribute("", GenerationMode.Automatic)]
		public Xareus.Samples.RelationEngineQueries.SampleType SimpleObjectSampleType;


		[ObjectPatternDescriptor("", GenerationMode.Automatic)]
		[ObjectPatternAttribute("ComplexObjectPattern","SampleType1","")]
		[ParameterDescriptorAttribute("", GenerationMode.Automatic)]
		public Xareus.Samples.RelationEngineQueries.SampleType ComplexObjectSampleType;


		[ObjectPatternAttribute("ComplexObjectPattern","SampleType2","")]
		[ParameterDescriptorAttribute("", GenerationMode.Automatic)]
		public Xareus.Samples.RelationEngineQueries.SampleType ComplexObjectSampleType2;

	 }
 }
