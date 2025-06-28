# Project Guidelines for AI Development

## AI Instructions

### Code Organization
- All script files MUST be C# (.cs) files
- All new script files MUST be placed in the `_Scripts` folder
- No namespaces should be used in new scripts
- Each script should be self-contained and follow Unity best practices

### Documentation Requirements
- Every new script must include XML documentation comments
- Include example usage in comments where appropriate
- Document all public methods and properties

### Naming Conventions
- Use PascalCase for class names and public members
- Use camelCase for private members and local variables
- Prefix private fields with underscore (_)

### Code Structure
- One class per file
- File name must match the primary class name
- Follow Unity's component-based architecture

### Best Practices
- Initialize variables in the Inspector where possible
- Use SerializeField for private fields that need Inspector exposure
- Implement proper cleanup in OnDestroy() when necessary
- Follow Unity's event functions order (Awake, Start, Update, etc.)

## Example Script Structure