namespace Shell;

using Shell.Enviroment;
using Shell.Parser;
using Shell.Command;
using Shell.Command.Integrated;
using Shell.Command.Hidden;

public class Interpreter
{
    private ShellEnvironment _env = new ShellEnvironment();
    private ShellParser _parser = new ShellParser();

    public Interpreter()
    {
        CommandResolver.RegisterBuiltIn("echo", (r, w, e) => new EchoCommand(r, w, e));
        CommandResolver.RegisterBuiltIn("ws", (r, w, e) => new WsCommand(r, w, e));
        CommandResolver.RegisterBuiltIn("cat", (r, w, e) => new CatCommand(r, w, e));

        CommandResolver.RegisterInternal("exit", () => new ExitCommand());
        CommandResolver.RegisterInternal("pwd", () => new PwdCommand());
    }

    public Result<bool> ProcessRequest(string? request)
    {
        return _parser
            .Parse(request, _env)
            .Then((x) => x.Run(_env))
            .Then(
                (x) =>
                {
                    try
                    {
                        var code = x.Result;
                        _env["?"] = code.ToString();
                        return ResultFactory.CreateResult<bool>(true);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Console.WriteLine(ex.ToString());
#endif
                        return ResultFactory.CreateResult<bool>(false);
                    }
                }
            );
    }
}
