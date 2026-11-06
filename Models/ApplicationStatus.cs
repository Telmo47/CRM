public enum ApplicationStatus{
    Submitted,
    Interviewing(stage1),
    Rejected,
    Offer,
    Testing(stage2)
}