using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoResiduos.Migrations
{
    /// <inheritdoc />
    public partial class DadosIniciais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TipoResiduos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorIdentificacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reciclavel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxaReciclagem = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MetodoProcessamento = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoResiduos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coletas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoResiduoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataColeta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Local = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coletas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coletas_TipoResiduos_TipoResiduoId",
                        column: x => x.TipoResiduoId,
                        principalTable: "TipoResiduos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coletas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Processamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColetaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoResiduoId = table.Column<int>(type: "int", nullable: false),
                    DataProcessamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Metodo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Eficiencia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantidadeProcessada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantidadeProduzida = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resultado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Processamentos_Coletas_ColetaId",
                        column: x => x.ColetaId,
                        principalTable: "Coletas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Processamentos_TipoResiduos_TipoResiduoId",
                        column: x => x.TipoResiduoId,
                        principalTable: "TipoResiduos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Processamentos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coletas_TipoResiduoId",
                table: "Coletas",
                column: "TipoResiduoId");

            migrationBuilder.CreateIndex(
                name: "IX_Coletas_UsuarioId",
                table: "Coletas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Processamentos_ColetaId",
                table: "Processamentos",
                column: "ColetaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Processamentos_TipoResiduoId",
                table: "Processamentos",
                column: "TipoResiduoId");

            migrationBuilder.CreateIndex(
                name: "IX_Processamentos_UsuarioId",
                table: "Processamentos",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Processamentos");

            migrationBuilder.DropTable(
                name: "Coletas");

            migrationBuilder.DropTable(
                name: "TipoResiduos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
