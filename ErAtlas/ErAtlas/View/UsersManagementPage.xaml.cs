using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErAtlas.ViewModels;

namespace ErAtlas.View;

public partial class UsersManagementPage : ContentPage
{
    public UsersManagementPage(UsersManagementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}