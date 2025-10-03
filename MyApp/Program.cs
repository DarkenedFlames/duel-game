using MyApp;

public class Program
{
    static void Main()
    {
        Player playerOne = new("Bob");
        Player playerTwo = new("Alice");
        TurnManager turnManager = new([playerOne, playerTwo]);
        turnManager.StartTurns();
    }
}
// Build Exe
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

// Major changes include new complex systems or breaking changes
// Minor changes include new features or significant improvements
// Patch changes include bug fixes or small tweaks

