namespace SSMLEditor.ViewModels
{
    using System.Threading.Tasks;
    using Catel.MVVM;

    public class VideoViewModel : ViewModelBase
    {
        public VideoViewModel(/* dependency injection here */)
        {
        }

        public override string Title { get { return "Video"; } }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // TODO: subscribe to events here
        }

        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            await base.CloseAsync();
        }
    }
}
