using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhereToSpendYourTime.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureTagUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemTag_Items_ItemId",
                table: "ItemTag");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemTag_Tags_TagId",
                table: "ItemTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemTag",
                table: "ItemTag");

            migrationBuilder.RenameTable(
                name: "ItemTag",
                newName: "ItemTags");

            migrationBuilder.RenameIndex(
                name: "IX_ItemTag_TagId",
                table: "ItemTags",
                newName: "IX_ItemTags_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemTags",
                table: "ItemTags",
                columns: new[] { "ItemId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTags_Items_ItemId",
                table: "ItemTags",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTags_Tags_TagId",
                table: "ItemTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemTags_Items_ItemId",
                table: "ItemTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemTags_Tags_TagId",
                table: "ItemTags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemTags",
                table: "ItemTags");

            migrationBuilder.RenameTable(
                name: "ItemTags",
                newName: "ItemTag");

            migrationBuilder.RenameIndex(
                name: "IX_ItemTags_TagId",
                table: "ItemTag",
                newName: "IX_ItemTag_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemTag",
                table: "ItemTag",
                columns: new[] { "ItemId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTag_Items_ItemId",
                table: "ItemTag",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTag_Tags_TagId",
                table: "ItemTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
