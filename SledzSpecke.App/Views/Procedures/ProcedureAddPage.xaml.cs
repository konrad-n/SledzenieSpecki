using SledzSpecke.App.ViewModels.Procedures;

namespace SledzSpecke.App.Views.Procedures;

public partial class ProcedureAddPage : ContentPage
{
    private readonly ProcedureAddViewModel _viewModel;

    public ProcedureAddPage(ProcedureAddViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.InitializeAsync();
    }
}
