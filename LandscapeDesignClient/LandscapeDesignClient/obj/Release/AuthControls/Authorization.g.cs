﻿#pragma checksum "..\..\..\AuthControls\Authorization.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "187268B1392E942800406F06C31020CA32823E1449F3C0DB3B88F22EE00F024C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LandscapeDesignClient.AuthControls;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace LandscapeDesignClient.AuthControls {
    
    
    /// <summary>
    /// Authorization
    /// </summary>
    public partial class Authorization : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 26 "..\..\..\AuthControls\Authorization.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gAuth;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\AuthControls\Authorization.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbEmail;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\AuthControls\Authorization.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbEmailError;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\AuthControls\Authorization.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox tbPass;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\AuthControls\Authorization.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbPassError;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\AuthControls\Authorization.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSignIn;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\AuthControls\Authorization.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnReg;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/LandscapeDesignClient;component/authcontrols/authorization.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\AuthControls\Authorization.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 20 "..\..\..\AuthControls\Authorization.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.SignIn_Executed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.gAuth = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.tbEmail = ((System.Windows.Controls.TextBox)(target));
            
            #line 47 "..\..\..\AuthControls\Authorization.xaml"
            this.tbEmail.LostFocus += new System.Windows.RoutedEventHandler(this.TbEmail_LostFocus);
            
            #line default
            #line hidden
            return;
            case 4:
            this.tbEmailError = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.tbPass = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 50 "..\..\..\AuthControls\Authorization.xaml"
            this.tbPass.LostFocus += new System.Windows.RoutedEventHandler(this.TbPass_LostFocus);
            
            #line default
            #line hidden
            
            #line 51 "..\..\..\AuthControls\Authorization.xaml"
            this.tbPass.PasswordChanged += new System.Windows.RoutedEventHandler(this.TbPass_PasswordChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.tbPassError = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.btnSignIn = ((System.Windows.Controls.Button)(target));
            return;
            case 8:
            this.btnReg = ((System.Windows.Controls.Button)(target));
            
            #line 64 "..\..\..\AuthControls\Authorization.xaml"
            this.btnReg.Click += new System.Windows.RoutedEventHandler(this.BtnReg_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

