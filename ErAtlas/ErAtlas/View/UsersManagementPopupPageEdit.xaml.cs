using ErAtlas.ViewModels;

namespace ErAtlas.View;

public partial class UsersManagementPopupPageEdit
{
    private readonly UsersManagementViewModel _viewModel;

    public UsersManagementPopupPageEdit(UsersManagementViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnCreerClicked(object sender, EventArgs e)
    {
        await _viewModel.CreationUtilisateurCommand.ExecuteAsync(null);

        if (_viewModel.IsSuccessVisible && !_viewModel.IsErrorVisible)
        {
            await Navigation.PopModalAsync();
        }
    }

    private async void OnAnnulerClicked(object sender, EventArgs e)
    {
        await _viewModel.AnnulerCreationCommand.ExecuteAsync(null);
        await Navigation.PopModalAsync();
    }
}

