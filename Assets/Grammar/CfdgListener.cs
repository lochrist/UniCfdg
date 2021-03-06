//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Cfdg.g4 by ANTLR 4.7.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="CfdgParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.2")]
[System.CLSCompliant(false)]
public interface ICfdgListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.contextfree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterContextfree([NotNull] CfdgParser.ContextfreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.contextfree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitContextfree([NotNull] CfdgParser.ContextfreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.statements"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatements([NotNull] CfdgParser.StatementsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.statements"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatements([NotNull] CfdgParser.StatementsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] CfdgParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] CfdgParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.startshape"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStartshape([NotNull] CfdgParser.StartshapeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.startshape"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStartshape([NotNull] CfdgParser.StartshapeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.background"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBackground([NotNull] CfdgParser.BackgroundContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.background"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBackground([NotNull] CfdgParser.BackgroundContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.design_rule"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDesign_rule([NotNull] CfdgParser.Design_ruleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.design_rule"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDesign_rule([NotNull] CfdgParser.Design_ruleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.replacements"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReplacements([NotNull] CfdgParser.ReplacementsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.replacements"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReplacements([NotNull] CfdgParser.ReplacementsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.replacement_loop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReplacement_loop([NotNull] CfdgParser.Replacement_loopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.replacement_loop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReplacement_loop([NotNull] CfdgParser.Replacement_loopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.replacement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReplacement([NotNull] CfdgParser.ReplacementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.replacement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReplacement([NotNull] CfdgParser.ReplacementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.modification"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterModification([NotNull] CfdgParser.ModificationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.modification"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitModification([NotNull] CfdgParser.ModificationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.adjustments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAdjustments([NotNull] CfdgParser.AdjustmentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.adjustments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAdjustments([NotNull] CfdgParser.AdjustmentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.adjustment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAdjustment([NotNull] CfdgParser.AdjustmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.adjustment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAdjustment([NotNull] CfdgParser.AdjustmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.color_adjustments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterColor_adjustments([NotNull] CfdgParser.Color_adjustmentsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.color_adjustments"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitColor_adjustments([NotNull] CfdgParser.Color_adjustmentsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.geom_adjustment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterGeom_adjustment([NotNull] CfdgParser.Geom_adjustmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.geom_adjustment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitGeom_adjustment([NotNull] CfdgParser.Geom_adjustmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.color_adjustment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterColor_adjustment([NotNull] CfdgParser.Color_adjustmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.color_adjustment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitColor_adjustment([NotNull] CfdgParser.Color_adjustmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CfdgParser.num"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNum([NotNull] CfdgParser.NumContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CfdgParser.num"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNum([NotNull] CfdgParser.NumContext context);
}
