// Model for the second table
using System.ComponentModel.DataAnnotations;

namespace Laya.Models
{
    public class Registration
    {   
        

    public int Id { get; set; }
    public string ? UserName { get; set; }
    public string? Password { get; set; }
    public string ? Email { get; set; }
    // Add fields for image and comment
    public string ? ImageName { get; set; }
    public byte[]? ImageContent { get; set; }
    public string ? ImageComment { get; set; }

    public string ? VideoName {get; set;}

    public byte[] ? VideoContent {get; set;}
    public string ? VideoComment {get; set;}
}


        
        

        }


    
