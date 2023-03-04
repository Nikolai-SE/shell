namespace shell.parser.primitive;

using System.Collections.Generic;
using shell.expression;

public class WsToken : Token {

    public WsToken(string ws = " ") {
        Original = ws;
        Type = TokenType.T_WS;
    }
    public WsToken(char ws) {
        Original = ws.ToString();
        Type = TokenType.T_WS;
    }
    public override Token? Join(Stack<Token> x) {
        if (x.Count == 0) {
            return null;
        }

        if (x.Peek().Type == TokenType.T_WS) {
            return new WsToken() { Original = x.Pop().Original + this.Original };
        }

        if (x.Peek().Type == TokenType.T_WORD) {
            var t = (WordToken)x.Pop();
            return new CommandToken(t.Value) { Original = t.Original + this.Original };
        }

        if (x.Peek().Type == TokenType.T_ASS) {
            var t = x.Pop();
            t.Original += Original;
            return t;
        }

        if (x.Peek().Type == TokenType.T_ASS_PRED) {
            var t = (AssingmentChunkToken)x.Pop();
            return new CommandToken(t.Value) { Original = t.Original + this.Original };
        }

        if (x.Peek().Type == TokenType.T_CMD) {
            var t = x.Pop();
            t.Original += Original;
            return t;
        }

        if (x.Peek().Type == TokenType.T_TMP) {
            var t = (TemplateToken)x.Pop();
            return t.Absorb(this);
        }

        return null;
    }

    public override Result<Expression> Render() {
        return ResultFactory.CreateError<Expression>("Sytax error!");
    }

    #if DEBUG
    public override string ToDebugString() {
        return "[WS: ' ']";
    }
    #endif
}

public class EndToken : WsToken {
    public EndToken() : base("") { }

    public override Result<Expression> Render() {
        return ResultFactory.CreateEmpty<Expression>();
    }

     #if DEBUG
    public override string ToDebugString() {
        return "[END]";
    }
    #endif
}
