using DoctorEverywhere.Enums;

namespace DoctorEverywhere.Domain;

/// <summary>
/// So here! For each day of week the doctor can refine the Starting Ending hours.
/// The important part will be in code where we extract these data for one doctor,
/// match it one date's appointments and show what is left
/// </summary>
/// Example data are following:
/*
| Id | DoctorId | DayOfWeek | ShiftStartTime | ShiftEndTime |
| -- | -------- | --------- | -------------- | ------------ |
| 1  | 1        | Monday    | 09:00:00       | 15:00:00     |
| 2  | 1        | Monday    | 19:00:00       | 21:00:00     |
| 3  | 1        | Tuesday   | 09:00:00       | 15:00:00     |
| 4  | 1        | Tuesday   | 19:00:00       | 21:00:00     |
| 5  | 1        | Wednesday | 09:00:00       | 15:00:00     |
| 6  | 1        | Wednesday | 19:00:00       | 21:00:00     |
| 7  | 1        | Thursday  | 09:00:00       | 15:00:00     |
| 8  | 1        | Thursday  | 19:00:00       | 21:00:00     |
| 9  | 1        | Friday    | 09:00:00       | 15:00:00     |
| 10 | 1        | Friday    | 19:00:00       | 21:00:00     |
 */
public class WorkingSchedule
{
    public int Id { get; set; }

    public DayOfWeekOption DayOfWeek { get; set; }

    public TimeSpan ShiftStartTime { get; set; }
    public TimeSpan ShiftEndTime { get; set; }

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }
}