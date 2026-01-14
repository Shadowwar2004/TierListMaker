namespace Domain
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
    }

    public class Element
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        
        
        public string Categorie { get; set; } = string.Empty; 
    }

    public class TierList
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        
        // Clé étrangère
        public int UtilisateurId { get; set; }
        public Utilisateur? Utilisateur { get; set; }

        // Liste des contenus
        public List<ContenuTierList> Contenus { get; set; } = new();
    }

    public class ContenuTierList
    {
        public int Id { get; set; }
        
        public int TierListId { get; set; }
        public TierList? TierList { get; set; }

        public int ElementId { get; set; }
        public Element? Element { get; set; }

        public string Notation { get; set; } = string.Empty; 
    }
}