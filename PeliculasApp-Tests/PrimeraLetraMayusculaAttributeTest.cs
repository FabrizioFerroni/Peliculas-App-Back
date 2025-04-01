using System.ComponentModel.DataAnnotations;
using PeliculasApp_Back.Validaciones;

namespace PeliculasApp_Tests;

[TestClass]
public sealed class PrimeraLetraMayusculaAttributeTest
{
    [TestMethod]
    [DataRow("")]
    [DataRow("  ")]
    [DataRow(null)]
    public void IsValid_ShouldReturnSuccess_IfTheValueIsEmpty(string valor)
    {
        // Preparar
        PrimeraLetraMayusculaAttribute? primeraLetraMayusculaAttribute = new PrimeraLetraMayusculaAttribute();
        
        ValidationContext? validationContext = new ValidationContext(new object());
        
        // Probar
        ValidationResult? result = primeraLetraMayusculaAttribute.GetValidationResult(valor, validationContext);
        
        // Verificar
        Assert.AreEqual(ValidationResult.Success, result);
    }
    
    [TestMethod]
    [DataRow("Fabrizio")]
    public void IsValid_ShouldReturnSuccess_IfTheFirstLetterIsCapitalized(string valor)
    {
        // Preparar
        PrimeraLetraMayusculaAttribute? primeraLetraMayusculaAttribute = new PrimeraLetraMayusculaAttribute();
        
        ValidationContext? validationContext = new ValidationContext(new object());
        
        // Probar
        ValidationResult? result = primeraLetraMayusculaAttribute.GetValidationResult(valor, validationContext);
        
        // Verificar
        Assert.AreEqual(ValidationResult.Success, result);
    }
    
    [TestMethod]
    [DataRow("fabrizio")]
    public void IsValid_ShouldReturnError_IfTheFirstLetterIsNotCapitalized(string valor)
    {
        // Preparar
        PrimeraLetraMayusculaAttribute? primeraLetraMayusculaAttribute = new PrimeraLetraMayusculaAttribute();
        
        ValidationContext? validationContext = new ValidationContext(new object());
        
        // Probar
        ValidationResult? result = primeraLetraMayusculaAttribute.GetValidationResult(valor, validationContext);
        
        // Verificar
        Assert.AreEqual("La primera letra debe ser mayúscula", result!.ErrorMessage);
    }
}