using LifeLine.Directory.Service.Client.Services.AdmissionStatus;
using LifeLine.Directory.Service.Client.Services.Department;
using LifeLine.Directory.Service.Client.Services.DocumentType;
using LifeLine.Directory.Service.Client.Services.EducationLevel;
using LifeLine.Directory.Service.Client.Services.PermitType;
using LifeLine.Directory.Service.Client.Services.Position.Factories;
using LifeLine.Directory.Service.Client.Services.Status;
using LifeLine.Employee.Service.Client.Services.Employee;
using LifeLine.Employee.Service.Client.Services.Employee.Assignment;
using LifeLine.Employee.Service.Client.Services.Employee.ContactInformation;
using LifeLine.Employee.Service.Client.Services.Employee.EducationDocument;
using LifeLine.Employee.Service.Client.Services.Employee.EmployeeSpecialtry;
using LifeLine.Employee.Service.Client.Services.Employee.PersonalDocument;
using LifeLine.Employee.Service.Client.Services.Employee.WorkPermit;
using LifeLine.Employee.Service.Client.Services.EmployeeType;
using LifeLine.Employee.Service.Client.Services.Gender;
using LifeLine.Employee.Service.Client.Services.Specialty;
using LifeLine.File.Service.Client;
using LifeLine.HrPanel.Desktop.Services.FilePreview;
using LifeLine.HrPanel.Desktop.Services.GenerateImage;
using LifeLine.HrPanel.Desktop.Services.GeneratePdf;
using LifeLine.User.Service.Client.ApiClients;
using LifeLine.User.Service.Client.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Client.Security.Abstraction;
using Shared.WPF.Services.Conversion;
using Shared.WPF.Services.FileDialog;
using Shared.WPF.Services.NavigationService.Pages;
using Shared.WPF.Services.NavigationService.Windows;

namespace LifeLine.HrPanel.Desktop.Ioc
{
    internal static class RegistrationService
    {
        public static IServiceCollection UseHrPanelServices(this ServiceCollection services, IConfiguration configuration)
        {
            services.UseFileService(configuration);

            services.AddSingleton<INavigationPage, NavigationPage>();
            services.AddSingleton<INavigationWindow, NavigationWindow>();

            services.AddSingleton<IFileDialogService, FileDialogService>();
            services.AddSingleton<IFilePreviewService, FilePreviewService>();
            services.AddSingleton<IGenerateImageService, GenerateImageService>();
            services.AddSingleton<IGeneratePdfService, GeneratePdfService>();

            services.AddSingleton<IImageCompressionService, ImageCompressionService>();
            services.AddSingleton<IDocumentConversionService, DocumentConversionService>();

            services.AddSingleton<IAuthorizationService>(sp =>
            {
                var navigationPage = sp.GetRequiredService<INavigationPage>();
                return new AuthorizationService
                    (
                        sp.GetRequiredService<IUserApiService>(),
                        sp.GetRequiredService<ISRPService>(),
                        sp.GetRequiredService<ITokenStorage>(),
                        sp.GetRequiredService<IUserContext>(),
                        onLogoutCallback: () => { navigationPage.CloseAll(); }
                    );
            });

            var employeeService = configuration.GetValue<string>("EmployeeService");
            string employeeHttp = "EmployeeServiceHttp";
            services.AddHttpClient(employeeHttp, client => client.BaseAddress = new Uri(employeeService!));

            services.AddHttpClient<IEmployeeService, EmployeeService>(employeeHttp);
            services.AddHttpClient<IGenderReadOnlyService, GenderService>(employeeHttp);
            services.AddHttpClient<IGenderService, GenderService>(employeeHttp);
            services.AddHttpClient<IEmployeeTypeReadOnlyService, EmployeeTypeService>(employeeHttp);
            services.AddHttpClient<ISpecialtyReadOnlyService, SpecialtyService>(employeeHttp);
            services.AddSingleton<IContactInformationApiServiceFactory, ContactInformationApiServiceFactory>();
            services.AddSingleton<IPersonalDocumentApiServiceFactory, PersonalDocumentApiServiceFactory>();
            services.AddSingleton<IEducationDocumentApiServiceFactory, EducationDocumentApiServiceFactory>();
            services.AddSingleton<IEmployeeSpecialtyApiServiceFactory, EmployeeSpecialtyApiServiceFactory>();
            services.AddSingleton<IWorkPermitApiServiceFactory, WorkPermitApiServiceFactory>();
            services.AddSingleton<IAssignmentApiServiceFactory, AssignmentApiServiceFactory>();

            var directoryService = configuration.GetValue<string>("DirectoryService");
            string directoryHttp = "DirectoryServiceHttp";
            services.AddHttpClient(directoryHttp, client => client.BaseAddress = new Uri(directoryService!));

            services.AddHttpClient<IDocumentTypeReadOnlyService, DocumentTypeService>(directoryHttp);
            services.AddHttpClient<IEducationLevelReadOnlyService, EducationLevelService>(directoryHttp);
            services.AddHttpClient<IAdmissionStatusReadOnlyService, AdmissionStatusService>(directoryHttp);
            services.AddHttpClient<IPermitTypeReadOnlyService, PermitTypeService>(directoryHttp);
            services.AddHttpClient<IStatusReadOnlyService, StatusService>(directoryHttp);
            services.AddHttpClient<IDepartmentReadOnlyService, DepartmentService>(directoryHttp);
            services.AddHttpClient<IDepartmentService, DepartmentService>(directoryHttp);
            services.AddSingleton<IPositionReadOnlyApiServiceFactory, PositionReadOnlyApiServiceFactory>();
            services.AddSingleton<IPositionApiServiceFactory, PositionApiServiceFactory>();

            return services;
        }
    }
}
