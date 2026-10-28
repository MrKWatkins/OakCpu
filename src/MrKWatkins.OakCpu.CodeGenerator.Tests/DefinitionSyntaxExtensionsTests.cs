using Microsoft.CodeAnalysis.CSharp;
using MrKWatkins.OakCpu.Definitions.Language.Ast;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class DefinitionSyntaxExtensionsTests : TestFixture
{
    [Test]
    public void Operator_SyntaxKind()
    {
        Operator.Add.SyntaxKind.Should().Equal(SyntaxKind.AddExpression);
        Operator.Not.SyntaxKind.Should().Equal(SyntaxKind.LogicalNotExpression);
        Operator.Assignment.SyntaxKind.Should().Equal(SyntaxKind.SimpleAssignmentExpression);
    }

    [Test]
    public void Operator_CompoundAssignmentSyntaxKind()
    {
        Operator.Add.CompoundAssignmentSyntaxKind.Should().Equal(SyntaxKind.AddAssignmentExpression);
        Operator.Equality.CompoundAssignmentSyntaxKind.Should().BeNull();

        foreach (var @operator in Operator.BinaryOperators.Values)
        {
            @operator.CompoundAssignmentSyntaxKind.HasValue.Should().Equal(@operator.HasCompoundAssignment);
        }
    }

    [Test]
    public void Expression_TypeSyntax() => new Number(1).TypeSyntax.ToNormalizedString().Should().Equal("byte");

    [Test]
    public void Access_Identifier()
    {
        new ArgumentAccess("temp").Identifier.ToString().Should().Equal("temp");

        var dataMember = Z80GeneratorContext.Configuration.UserDefinedDataMembers.Values.First();
        new DataMemberAccess(dataMember).Identifier.ToString().Should().Equal(dataMember.FieldName);

        var register = Z80GeneratorContext.Configuration.Registers["AF'"];
        new RegisterAccess(register).Identifier.ToString().Should().Equal(register.FieldName);
    }

    [Test]
    public void Register_TypeSyntax() => Z80GeneratorContext.Configuration.Registers["HL"].TypeSyntax.ToNormalizedString().Should().Equal("ushort");

    [Test]
    public void Function_TypeSyntax() => PreDefinedFunction.Signed.TypeSyntax.ToNormalizedString().Should().Equal("sbyte");

    [Test]
    public void DataMember_TypeSyntax() => PreDefinedDataMember.Data.TypeSyntax.ToNormalizedString().Should().Equal("byte");
}