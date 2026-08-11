{
  description = "fodinfo .NET development shell";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs =
    { nixpkgs, ... }:
    let
      supportedSystems = [
        "x86_64-linux"
        "aarch64-linux"
        "aarch64-darwin"
      ];

      forEachSupportedSystem = nixpkgs.lib.genAttrs supportedSystems;
    in
    {
      devShells = forEachSupportedSystem (
        system:
        let
          pkgs = import nixpkgs { inherit system; };
          inherit (pkgs) lib stdenv;
          dotnetSdk = pkgs.dotnetCorePackages.sdk_10_0;
        in
        {
          default = pkgs.mkShellNoCC {
            packages = [
              dotnetSdk
              pkgs.fsautocomplete
            ]
            ++ lib.optionals (!(stdenv.hostPlatform.isDarwin && stdenv.hostPlatform.isAarch64)) [
              pkgs.csharp-ls
            ];

            env = {
              DOTNET_CLI_TELEMETRY_OPTOUT = "1";
              DOTNET_NOLOGO = "1";
              DOTNET_ROOT = "${dotnetSdk}";
            };
          };
        }
      );
    };
}
