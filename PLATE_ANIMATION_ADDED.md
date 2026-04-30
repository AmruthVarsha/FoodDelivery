# Plate Animation - Customer Dashboard

## Feature Added: Animated Plates Sliding In

### What Was Added:
Two animated food plate images that slide in from both sides when the customer dashboard loads, creating an engaging entrance animation.

### Implementation Details:

#### 1. HTML Structure
**File**: `FrontEndNew/src/app/features/customer/dashboard/dashboard.component.html`

Added two plate images positioned on left and right sides of the hero section:

```html
<!-- Left Plate - Slides in from left -->
<div class="absolute left-0 top-1/2 -translate-y-1/2 w-32 h-32 hidden lg:block animate-slide-in-left">
  <img alt="Food Plate Left" class="w-full h-full object-contain drop-shadow-2xl"
    src="[food-plate-image-url]" />
</div>

<!-- Right Plate - Slides in from right -->
<div class="absolute right-0 top-1/2 -translate-y-1/2 w-32 h-32 hidden lg:block animate-slide-in-right">
  <img alt="Food Plate Right" class="w-full h-full object-contain drop-shadow-2xl"
    src="[food-plate-image-url]" />
</div>
```

**Positioning**:
- `absolute`: Positioned absolutely within hero section
- `left-0` / `right-0`: Aligned to left/right edges
- `top-1/2 -translate-y-1/2`: Vertically centered
- `w-32 h-32`: 128px × 128px size
- `hidden lg:block`: Only visible on large screens (desktop)

#### 2. CSS Animations
**File**: `FrontEndNew/src/app/features/customer/dashboard/dashboard.component.css`

**Slide-in from Left Animation**:
```css
@keyframes slideInLeft {
  0% {
    transform: translateX(-200%) translateY(-50%);
    opacity: 0;
  }
  100% {
    transform: translateX(0) translateY(-50%);
    opacity: 1;
  }
}
```

**Slide-in from Right Animation**:
```css
@keyframes slideInRight {
  0% {
    transform: translateX(200%) translateY(-50%);
    opacity: 0;
  }
  100% {
    transform: translateX(0) translateY(-50%);
    opacity: 1;
  }
}
```

**Floating Animation** (after slide-in):
```css
@keyframes float {
  0%, 100% {
    transform: translateY(-50%) translateX(0);
  }
  50% {
    transform: translateY(-50%) translateX(0) translateY(-10px);
  }
}
```

**Animation Classes**:
```css
.animate-slide-in-left {
  animation: slideInLeft 1.2s cubic-bezier(0.34, 1.56, 0.64, 1) forwards,
             float 3s ease-in-out 1.2s infinite;
}

.animate-slide-in-right {
  animation: slideInRight 1.2s cubic-bezier(0.34, 1.56, 0.64, 1) forwards,
             float 3s ease-in-out 1.2s infinite;
}
```

### Animation Timeline:

1. **0s - 1.2s**: Plates slide in from both sides
   - Left plate slides from left edge (-200%)
   - Right plate slides from right edge (200%)
   - Both fade in from opacity 0 to 1
   - Uses bounce easing: `cubic-bezier(0.34, 1.56, 0.64, 1)`

2. **1.2s onwards**: Continuous floating animation
   - Plates gently float up and down
   - 3-second cycle
   - Creates a subtle, engaging effect

### Visual Effects:

1. **Drop Shadow**: `drop-shadow-2xl` for depth
2. **Object Contain**: Images maintain aspect ratio
3. **Smooth Easing**: Bounce effect on entry
4. **Continuous Motion**: Floating keeps plates alive

### Responsive Design:

- **Desktop (lg and above)**: Plates visible and animated
- **Mobile/Tablet**: Plates hidden (`hidden lg:block`)
- Prevents clutter on smaller screens

### Performance:

- **GPU Acceleration**: Uses `transform` for smooth animation
- **Efficient**: Only animates on page load
- **No JavaScript**: Pure CSS animations
- **Lightweight**: Minimal performance impact

### Customization Options:

You can easily customize:

1. **Size**: Change `w-32 h-32` to any size
2. **Duration**: Modify `1.2s` in animation
3. **Easing**: Change `cubic-bezier` values
4. **Float Speed**: Adjust `3s` in float animation
5. **Images**: Replace image URLs with different plates
6. **Position**: Adjust `left-0`, `right-0` values

### Browser Support:

- ✅ Chrome/Edge (all versions)
- ✅ Firefox (all versions)
- ✅ Safari (all versions)
- ✅ Opera (all versions)

### Files Modified:

1. `FrontEndNew/src/app/features/customer/dashboard/dashboard.component.html`
2. `FrontEndNew/src/app/features/customer/dashboard/dashboard.component.css`

### Result:

When users land on the customer dashboard:
1. Two food plates dramatically slide in from both sides
2. They bounce slightly as they settle into position
3. They continue to float gently, creating a lively, appetizing atmosphere
4. The animation draws attention to the search bar in the center
5. Creates a memorable, engaging first impression

This animation enhances the user experience by:
- ✅ Creating visual interest
- ✅ Drawing attention to the search functionality
- ✅ Establishing a fun, food-focused brand personality
- ✅ Making the page feel dynamic and alive
- ✅ Improving perceived performance (users are entertained while content loads)
