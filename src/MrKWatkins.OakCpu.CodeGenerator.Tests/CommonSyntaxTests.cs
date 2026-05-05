using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class CommonSyntaxTests
{
    [Test]
    public void BoolType()
    {
        var result = CommonSyntax.BoolType;
        result.ToNormalizedString().Should().Equal("bool");
    }

    [Test]
    public void ByteType()
    {
        var result = CommonSyntax.ByteType;
        result.ToNormalizedString().Should().Equal("byte");
    }

    [Test]
    public void IntType()
    {
        var result = CommonSyntax.IntType;
        result.ToNormalizedString().Should().Equal("int");
    }

    [Test]
    public void UShortType()
    {
        var result = CommonSyntax.UShortType;
        result.ToNormalizedString().Should().Equal("ushort");
    }

    [Test]
    public void VoidType()
    {
        var result = CommonSyntax.VoidType;
        result.ToNormalizedString().Should().Equal("void");
    }

    [Test]
    public void Field()
    {
        var result = CommonSyntax.Field;
        result.ValueText.Should().Equal("field");
    }

    [Test]
    public void Internal()
    {
        var result = CommonSyntax.Internal;
        result.ValueText.Should().Equal("internal");
    }

    [Test]
    public void Partial()
    {
        var result = CommonSyntax.Partial;
        result.ValueText.Should().Equal("partial");
    }

    [Test]
    public void Private()
    {
        var result = CommonSyntax.Private;
        result.ValueText.Should().Equal("private");
    }

    [Test]
    public void Public()
    {
        var result = CommonSyntax.Public;
        result.ValueText.Should().Equal("public");
    }

    [Test]
    public void ReadOnly()
    {
        var result = CommonSyntax.ReadOnly;
        result.ValueText.Should().Equal("readonly");
    }

    [Test]
    public void Ref()
    {
        var result = CommonSyntax.Ref;
        result.ValueText.Should().Equal("ref");
    }

    [Test]
    public void Sealed()
    {
        var result = CommonSyntax.Sealed;
        result.ValueText.Should().Equal("sealed");
    }

    [Test]
    public void Semicolon()
    {
        var result = CommonSyntax.Semicolon;
        result.ValueText.Should().Equal(";");
    }

    [Test]
    public void Static()
    {
        var result = CommonSyntax.Static;
        result.ValueText.Should().Equal("static");
    }

    [Test]
    public void Unsafe()
    {
        var result = CommonSyntax.Unsafe;
        result.ValueText.Should().Equal("unsafe");
    }

    [Test]
    public void InitializeVariableStatement_WithVarType()
    {
        var value = LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(42));
        var result = CommonSyntax.InitializeVariableStatement("myVariable", value);

        result.ToNormalizedString().Should().Equal("var myVariable = 42;");
    }

    [Test]
    public void InitializeVariableStatement_WithSpecificType()
    {
        var value = LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(42));
        var type = PredefinedType(Token(SyntaxKind.IntKeyword));
        var result = CommonSyntax.InitializeVariableStatement("myVariable", value, type);

        result.ToNormalizedString().Should().Equal("int myVariable = 42;");
    }

    [Test]
    public void CreateArrayGetWithoutBoundsCheck()
    {
        var requiredUsings = new RequiredUsings();
        var array = IdentifierName("values");
        var index = IdentifierName("i");

        var result = CommonSyntax.CreateArrayGetWithoutBoundsCheck(requiredUsings, array, index);

        requiredUsings.Count.Should().Equal(2);
        requiredUsings.Contains("System.Runtime.CompilerServices").Should().BeTrue();
        requiredUsings.Contains("System.Runtime.InteropServices").Should().BeTrue();
        result.ToNormalizedString().Should().Equal("Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(values), i)");
    }

    [Test]
    public void CreateMethodImplAttribute_WithEnum()
    {
        var requiredUsings = new RequiredUsings();
        var result = CommonSyntax.CreateMethodImplAttribute(requiredUsings, MethodImplOptions.AggressiveInlining);

        requiredUsings.Count.Should().Equal(1);
        requiredUsings.Contains("System.Runtime.CompilerServices").Should().BeTrue();
        result.ToNormalizedString().Should().Equal("MethodImpl(MethodImplOptions.AggressiveInlining)");
    }

    [Test]
    public void CreateMethodImplAttribute_WithString()
    {
        var requiredUsings = new RequiredUsings();
        var result = CommonSyntax.CreateMethodImplAttribute(requiredUsings, "AggressiveInlining");

        requiredUsings.Count.Should().Equal(1);
        requiredUsings.Contains("System.Runtime.CompilerServices").Should().BeTrue();
        result.ToNormalizedString().Should().Equal("MethodImpl(MethodImplOptions.AggressiveInlining)");
    }

    [Test]
    public void EmulatorMemberIdentifier()
    {
        var result = CommonSyntax.EmulatorMemberIdentifier("Memory");
        result.ToNormalizedString().Should().Equal("emulator.Memory");
    }

    [Test]
    public void CreateEmulatorArgument()
    {
        var result = CommonSyntax.CreateEmulatorArgument();
        result.ToNormalizedString().Should().Equal("emulator");
    }

    [Test]
    public void GenerateBinaryLiteral()
    {
        var result = CommonSyntax.GenerateBinaryLiteral(42);
        result.Text.Should().Equal("0b00101010");
        result.Value.Should().Equal(42);
    }

    [Test]
    public void GenerateBinaryLiteralExpression()
    {
        var result = CommonSyntax.GenerateBinaryLiteralExpression(42);
        result.ToNormalizedString().Should().Equal("0b00101010");
    }

    [Test]
    public void GenerateNumericLiteralExpression()
    {
        var result = CommonSyntax.GenerateNumericLiteralExpression(123);
        result.ToNormalizedString().Should().Equal("123");
    }

    [Test]
    public void CreateAssignEmulatorFieldExpression()
    {
        var result = CommonSyntax.CreateAssignEmulatorFieldExpression();
        result.ToNormalizedString().Should().Equal("this.emulator = emulator;");
    }

    [Test]
    public void CreateNewObjectAndAssignToProperty()
    {
        var arg1 = IdentifierName("arg1");
        var arg2 = LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(42));
        var result = CommonSyntax.CreateNewObjectAndAssignToProperty("MyProperty", "MyClass", arg1, arg2);

        result.ToNormalizedString().Should().Equal("MyProperty = new MyClass(arg1, 42);");
    }

    [Test]
    public void CreateNewObjectAndAssignToProperty_NoArguments()
    {
        var result = CommonSyntax.CreateNewObjectAndAssignToProperty("MyProperty", "MyClass");
        result.ToNormalizedString().Should().Equal("MyProperty = new MyClass();");
    }

}