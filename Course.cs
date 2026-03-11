namespace Web_Sims.Models;

public class Course
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Credits { get; set; }
    // Username of the instructor responsible for this course (e.g. "lecturer1")
    public string Instructor { get; set; }
}
