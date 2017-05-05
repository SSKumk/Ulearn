﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Models
{
	public class XQueueExerciseSubmission
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int SubmissionId { get; set; }
		public virtual UserExerciseSubmission Submission { get; set; }
		
		public int WatcherId { get; set; }
		public virtual XQueueWatcher Watcher { get; set; }
		
		[Required(AllowEmptyStrings = true)]
		public string XQueueHeader { get; set; }
	}
}
