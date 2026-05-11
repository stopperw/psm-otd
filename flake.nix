{
  description = "PSM-OTD development shell";
  inputs = {
    nixpkgs.url = "nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };
  outputs = {
    nixpkgs,
    flake-utils,
    ...
  }:
    flake-utils.lib.eachDefaultSystem (
      system: let
        pkgs = import nixpkgs {inherit system;};
        dotnet-sdk = pkgs.dotnet-sdk_8;
      in {
        devShells = {
          default = pkgs.mkShell {
            buildInputs = [dotnet-sdk pkgs.omnisharp-roslyn];
          };
        };
      }
    );
}

