using System.Text;

namespace BadgerSerialization.Generator;

public class CodeGenerator
{
    public const string SpcIndent = "    ";
    private readonly StringBuilder _codeBuilder;
    private string _currentIndentLevel;

    public CodeGenerator()
    {
        _codeBuilder = new StringBuilder();
        _currentIndentLevel = "";
    }

    public void AddLine()
        => _codeBuilder.AppendLine();

    public void AddLine(string line)
        => _codeBuilder.AppendLine(_currentIndentLevel + line);

    public void IncreaseIndentLevel()
        => _currentIndentLevel += SpcIndent;

    public void DecreaseIndentLevel()
        => _currentIndentLevel = _currentIndentLevel.Substring(0, _currentIndentLevel.Length - SpcIndent.Length);

    public override string ToString()
        => _codeBuilder.ToString();

    public void EnterScope(string decl = "")
    {
        if (decl != "")
            AddLine(decl);

        AddLine("{");
        IncreaseIndentLevel();
    }

    public void LeaveScope()
    {
        DecreaseIndentLevel();
        AddLine("}");
    }
}