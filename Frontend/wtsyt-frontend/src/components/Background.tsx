interface BackgroundProps {
  solid?: boolean;
  disabled?: boolean;
}

export default function Background({ solid = false, disabled = false }: BackgroundProps) {
  if (disabled) return null;
  
  if (solid) {
    return (
      <div className="pointer-events-none fixed inset-0 z-0 overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-r from-gray-900 via-gray-800 to-black" />
      </div>
    );
  }
  
  return (
    <div className="pointer-events-none fixed inset-0 z-0 overflow-hidden bg-aurora">
      <div className="absolute inset-0 animate-gradient bg-[length:400%_400%]
        bg-gradient-to-r from-gray-950 via-indigo-950 to-black opacity-90" />
    </div>
  );
}