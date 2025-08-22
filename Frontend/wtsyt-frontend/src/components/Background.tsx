interface BackgroundProps {
  solid?: boolean;
}

export default function Background({ solid = false }: BackgroundProps) {
  if (solid) {
    return (
      <div className="pointer-events-none fixed inset-0 z-0">
        <div className="absolute inset-0 bg-gradient-to-r from-gray-900 via-gray-800 to-black" />
      </div>
    );
  }
  
  return (
    <div className="pointer-events-none fixed inset-0 z-0 bg-aurora">
      <div className="absolute inset-0 animate-gradient bg-[length:400%_400%]
        bg-gradient-to-r from-gray-950 via-indigo-950 to-black opacity-90" />
    </div>
  );
}