using CBA;

public class Program
{
    static void Main()
    {
        // --- Create the world singleton ---
        World world = new();

        // --- Create player entities ---
        var playerOne = new PlayerEntity();
        var playerTwo = new PlayerEntity();

        // --- Add PlayerData ---
        playerOne.AddComponent(new PlayerData(playerOne, "Bob"));
        playerTwo.AddComponent(new PlayerData(playerTwo, "Alice"));

        // --- Add Stats and Resources ---
        var playerOneStats = new StatsComponent(playerOne);
        playerOne.AddComponent(playerOneStats);
        playerOne.AddComponent(new ResourcesComponent(playerOne, playerOneStats));

        var playerTwoStats = new StatsComponent(playerTwo);
        playerTwo.AddComponent(playerTwoStats);
        playerTwo.AddComponent(new ResourcesComponent(playerTwo, playerTwoStats));

        // --- Add TakesTurns ---
        playerOne.AddComponent(new TakesTurns(playerOne));
        playerTwo.AddComponent(new TakesTurns(playerTwo));

        playerOne.SubscribeAll();
        playerTwo.SubscribeAll();

        // --- Start game loop ---
        world.TurnManager.StartGameLoop();
    }
}

// --- Concrete PlayerEntity class ---
namespace CBA
{
    public class PlayerEntity() : Entity() { }
}




// ======== Development Notes ========

// Build Executable
// dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

// Count Lines of Code
// (gci -include *.cs,*.xaml -recurse | select-string .).Count

// *Git workflow*

// Start of work
// git pull origin main     Sync local with GitHub
// git status               Double-check sync status

// Each feature/fix
// git add .                Stage all changes
// git commit -m "message"  Commit changes with a message
// git push origin main     Push changes to GitHub

// Time for a new release (1.x.x for major, x.1.x for minor, x.x.1 for patch)
// repeat "Each feature/fix" steps to catch remaining changes
// git tag -a v0.1.0 -m "Alpha milestone: first full duel loop"
// git push origin v0.1.0
// go to GitHub and create a new release with notes

// Major changes include new complex systems or breaking changes
// Minor changes include new features or significant improvements
// Patch changes include bug fixes or small tweaks


