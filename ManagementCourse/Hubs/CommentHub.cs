using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ManagementCourse.Hubs
{
    public class CommentHub : Hub
    {
        public async Task JoinLessonGroup(int lessonId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Lesson_{lessonId}");
        }

        public async Task LeaveLessonGroup(int lessonId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Lesson_{lessonId}");
        }

        public async Task JoinUserGroup(int employeeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{employeeId}");
        }

        public async Task LeaveUserGroup(int employeeId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{employeeId}");
        }
    }
}
