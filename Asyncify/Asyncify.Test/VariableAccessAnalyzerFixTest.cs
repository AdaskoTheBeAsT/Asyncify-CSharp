using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Asyncify.Test
{
    public class VariableAccessAnalyzerFixTest : BasAnalyzerFixTest
    {
        [TestMethod]
        public void CanFindMethodNotUsingTapWithVariable()
        {
            var expected = GetResultWithLocation(7, 17);
            VerifyCodeWithReturn(@"
    public void Test()
    {
        var temp = CallAsync();
        var result = temp.Result;
    }", expected);
            VerifyCodeFixWithReturn(@"
    public void Test()
    {
        var temp = CallAsync();
        var result = temp.Result;
    }", @"
    public async System.Threading.Tasks.Task Test()
    {
        var temp = CallAsync();
        var result = await temp;
    }", expected);
            VerifyCodeNoReturn(@"
    public void Test()
    {
        var temp = CallAsync();
    }", expected);
        }

        [TestMethod]
        public void CanFindMethodNotUsingTapWithVariableInBraces()
        {
            var expected = GetResultWithLocation(7, 17);
            VerifyCodeWithReturn(@"
    public void Test()
    {
        var temp = CallAsync();
        var result = ((Task<int>)temp).Result;
    }", expected);
            VerifyCodeFixWithReturn(@"
    public void Test()
    {
        var temp = CallAsync();
        var result = ((Task<int>)temp).Result;
    }", @"
    public async System.Threading.Tasks.Task Test()
    {
        var temp = CallAsync();
        var result = await ((Task<int>)temp);
    }", expected);
            VerifyCodeNoReturn(@"
    public void Test()
    {
        CallAsync();
    }", expected);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new VariableAccessFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new VariableAccessAnalyzer();
        }

        public override string DiagnosticId => VariableAccessAnalyzer.DiagnosticId;
    }
}