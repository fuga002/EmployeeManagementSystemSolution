namespace EMS.ClientExample.ApplicationStates;

public class DepartmentState
{
    public Action? GeneralDepartmentAction { get; set; }
    public bool ShowGeneralDepartment { get; set; }

    public void GeneralDepartmentClicked()
    {
        ResetAllDepartments();
        ShowGeneralDepartment = true;
        GeneralDepartmentAction?.Invoke();
    }

    private void ResetAllDepartments()
    {
        throw new NotImplementedException();
    }
}