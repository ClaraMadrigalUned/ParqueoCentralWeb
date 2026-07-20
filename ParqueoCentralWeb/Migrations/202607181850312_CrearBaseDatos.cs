namespace ParqueoCentralWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CrearBaseDatos : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EspacioEstacionamientoes",
                c => new
                    {
                        IdEspacio = c.Int(nullable: false, identity: true),
                        CodigoEspacio = c.String(nullable: false, maxLength: 10),
                        TipoEspacio = c.String(nullable: false),
                        Estado = c.String(nullable: false),
                        Activo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdEspacio);
            
            CreateTable(
                "dbo.MovimientoEstacionamientoes",
                c => new
                    {
                        IdMovimiento = c.Int(nullable: false, identity: true),
                        IdVehiculo = c.Int(nullable: false),
                        IdEspacio = c.Int(nullable: false),
                        FechaHoraEntrada = c.DateTime(nullable: false),
                        FechaHoraSalida = c.DateTime(),
                        EstadoMovimiento = c.String(nullable: false),
                        MontoCobrado = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UsuarioRegistro = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.IdMovimiento)
                .ForeignKey("dbo.EspacioEstacionamientoes", t => t.IdEspacio, cascadeDelete: true)
                .ForeignKey("dbo.Vehiculoes", t => t.IdVehiculo, cascadeDelete: true)
                .Index(t => t.IdVehiculo)
                .Index(t => t.IdEspacio);
            
            CreateTable(
                "dbo.Vehiculoes",
                c => new
                    {
                        IdVehiculo = c.Int(nullable: false, identity: true),
                        Placa = c.String(nullable: false, maxLength: 15),
                        TipoVehiculo = c.String(nullable: false),
                        Propietario = c.String(nullable: false, maxLength: 100),
                        Contacto = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.IdVehiculo);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MovimientoEstacionamientoes", "IdVehiculo", "dbo.Vehiculoes");
            DropForeignKey("dbo.MovimientoEstacionamientoes", "IdEspacio", "dbo.EspacioEstacionamientoes");
            DropIndex("dbo.MovimientoEstacionamientoes", new[] { "IdEspacio" });
            DropIndex("dbo.MovimientoEstacionamientoes", new[] { "IdVehiculo" });
            DropTable("dbo.Vehiculoes");
            DropTable("dbo.MovimientoEstacionamientoes");
            DropTable("dbo.EspacioEstacionamientoes");
        }
    }
}
