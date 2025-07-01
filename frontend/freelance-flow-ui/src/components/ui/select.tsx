import React, { createContext, useContext, useState, ReactNode } from 'react';
import { ChevronDown } from 'lucide-react';
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

// Create cn function inline to avoid import issues
function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

interface SelectContextType {
  value: string;
  onValueChange: (value: string) => void;
  open: boolean;
  setOpen: (open: boolean) => void;
}

const SelectContext = createContext<SelectContextType | undefined>(undefined);

const useSelect = () => {
  const context = useContext(SelectContext);
  if (!context) {
    throw new Error('Select components must be used within a Select');
  }
  return context;
};

interface SelectProps {
  children: ReactNode;
  value?: string;
  defaultValue?: string;
  onValueChange?: (value: string) => void;
}

interface SelectTriggerProps {
  children: ReactNode;
  className?: string;
}

interface SelectContentProps {
  children: ReactNode;
  className?: string;
}

interface SelectItemProps {
  children: ReactNode;
  value: string;
  className?: string;
}

interface SelectValueProps {
  placeholder?: string;
  className?: string;
}

export const Select: React.FC<SelectProps> = ({ 
  children, 
  value: controlledValue, 
  defaultValue, 
  onValueChange 
}) => {
  const [internalValue, setInternalValue] = useState(defaultValue || '');
  const [open, setOpen] = useState(false);
  
  const value = controlledValue !== undefined ? controlledValue : internalValue;
  
  const handleValueChange = (newValue: string) => {
    if (onValueChange) {
      onValueChange(newValue);
    } else {
      setInternalValue(newValue);
    }
    setOpen(false);
  };

  return (
    <SelectContext.Provider value={{ value, onValueChange: handleValueChange, open, setOpen }}>
      <div className="relative">
        {children}
      </div>
    </SelectContext.Provider>
  );
};

export const SelectTrigger: React.FC<SelectTriggerProps> = ({ children, className }) => {
  const { open, setOpen } = useSelect();
  
  return (
    <button
      type="button"
      className={cn(
        "flex h-10 w-full items-center justify-between rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50",
        className
      )}
      onClick={() => setOpen(!open)}
    >
      {children}
      <ChevronDown className="h-4 w-4 opacity-50" />
    </button>
  );
};

export const SelectContent: React.FC<SelectContentProps> = ({ children, className }) => {
  const { open } = useSelect();
  
  if (!open) return null;
  
  return (
    <div className={cn(
      "absolute top-full z-50 w-full min-w-[8rem] overflow-hidden rounded-md border bg-popover p-1 text-popover-foreground shadow-md",
      className
    )}>
      {children}
    </div>
  );
};

export const SelectItem: React.FC<SelectItemProps> = ({ children, value, className }) => {
  const { onValueChange } = useSelect();
  
  return (
    <div
      className={cn(
        "relative flex w-full cursor-default select-none items-center rounded-sm py-1.5 pl-8 pr-2 text-sm outline-none hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground cursor-pointer",
        className
      )}
      onClick={() => onValueChange(value)}
    >
      {children}
    </div>
  );
};

export const SelectValue: React.FC<SelectValueProps> = ({ placeholder, className }) => {
  const { value } = useSelect();
  
  return (
    <span className={cn("block truncate", className)}>
      {value || placeholder}
    </span>
  );
};