using RunMate.RunMate.Domain.Enums;
using RunMate.UserService.RunMate.Domain.Entities;

namespace RunMate.Domain.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; } 
        public bool IsActive { get; set; } = true; 
        public string? RefreshToken { get; set; } 
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public virtual ICollection<EventEntity> OrganizedEvents { get; set; }
        public virtual ICollection<EventParticipantEntity> EventParticipations { get; set; }

        // Adicionando coleções para o Feed de Publicações
        public virtual ICollection<PublicationEntity> Publications { get; set; }
        public virtual ICollection<PublicationCommentEntity> Comments { get; set; }
        public virtual ICollection<PublicationLikeEntity> Likes { get; set; }
        public virtual ICollection<UserFriendEntity> Friends { get; set; } = new List<UserFriendEntity>();
        public virtual ICollection<UserFriendEntity> FriendOf { get; set; } = new List<UserFriendEntity>();

        //// Perfil físico e foto
        //public double? Height { get; set; } // Altura em cm
        //public double? Weight { get; set; } // Peso em kg
        //public string? ProfilePicture { get; set; }
        //public string? Biography { get; set; }

        //// Sistema de gamificação
        //public int Level { get; set; } = 1;
        //public int ExperiencePoints { get; set; } = 0;
        //public int Ranking { get; set; } = 0;

        //// Estatísticas de corrida
        //public int TotalRuns { get; set; } = 0;
        //public double TotalDistance { get; set; } = 0; // em km
        //public TimeSpan TotalRunTime { get; set; } = TimeSpan.Zero;
        //public double AveragePace { get; set; } = 0; // min/km

        //// Configurações e preferências
        //public string? Location { get; set; } // Para informações meteorológicas
        //public bool NotificationsEnabled { get; set; } = true;
        //public bool AudioAssistantEnabled { get; set; } = true;
        //public AudioAssistantType PreferredAudioType { get; set; } = AudioAssistantType.Standard;

        //// Configurações de privacidade
        //public PrivacyLevel ProfilePrivacy { get; set; } = PrivacyLevel.Public;
        //public PrivacyLevel ActivityPrivacy { get; set; } = PrivacyLevel.Friends;

        //// Navegação para relacionamentos
        //public virtual ICollection<UserFriendEntity>? Friends { get; set; }
        //public virtual ICollection<GoalEntity>? Goals { get; set; }
        //public virtual ICollection<ChallengeEntity>? Challenges { get; set; }
        //public virtual ICollection<GroupEntity>? Groups { get; set; }
        //public virtual ICollection<EventEntity>? Events { get; set; }
        //public virtual ICollection<RoutineEntity>? Routines { get; set; }
        //public virtual ICollection<PostEntity>? Posts { get; set; }
    }
}
