public interface IMenuManager
{
    Menu GetCurrentMenu();
    void SetMenu(Menu menu);
}

public class MenuManager : IMenuManager
{
    private Menu menu;

    public MenuManager(Menu startingMenu)
    {
        this.menu = startingMenu;
        this.menu.Display();
    }

    public Menu GetCurrentMenu()
    {
        return menu;
    }

    public void SetMenu(Menu menu)
    {
        this.menu = menu;
        this.menu.Display();
    }
}
