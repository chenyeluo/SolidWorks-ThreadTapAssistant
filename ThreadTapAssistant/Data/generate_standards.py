import json

def get_metric_coarse_specs():
    specs_data = [
        ("M1.6", 1.6, 0.35, 1.25, [(1.221, 1.321), (1.221, 1.350), (1.221, 1.380)]),
        ("M2", 2.0, 0.40, 1.60, [(1.567, 1.679), (1.567, 1.707), (1.567, 1.740)]),
        ("M2.5", 2.5, 0.45, 2.05, [(2.013, 2.138), (2.013, 2.169), (2.013, 2.200)]),
        ("M3", 3.0, 0.50, 2.50, [(2.459, 2.599), (2.459, 2.639), (2.459, 2.670)]),
        ("M4", 4.0, 0.70, 3.30, [(3.242, 3.422), (3.242, 3.466), (3.242, 3.510)]),
        ("M5", 5.0, 0.80, 4.20, [(4.134, 4.334), (4.134, 4.384), (4.134, 4.430)]),
        ("M6", 6.0, 1.00, 5.00, [(4.917, 5.153), (4.917, 5.217), (4.917, 5.280)]),
        ("M8", 8.0, 1.25, 6.80, [(6.647, 6.912), (6.647, 6.982), (6.647, 7.050)]),
        ("M10", 10.0, 1.50, 8.50, [(8.376, 8.676), (8.376, 8.756), (8.376, 8.840)]),
        ("M12", 12.0, 1.75, 10.20, [(10.106, 10.441), (10.106, 10.531), (10.106, 10.630)]),
        ("M14", 14.0, 2.00, 12.00, [(11.835, 12.210), (11.835, 12.310), (11.835, 12.420)]),
        ("M16", 16.0, 2.00, 14.00, [(13.835, 14.210), (13.835, 14.310), (13.835, 14.420)]),
        ("M18", 18.0, 2.50, 15.50, [(15.294, 15.744), (15.294, 15.864), (15.294, 16.000)]),
        ("M20", 20.0, 2.50, 17.50, [(17.294, 17.744), (17.294, 17.864), (17.294, 18.000)]),
        ("M22", 22.0, 2.50, 19.50, [(19.294, 19.744), (19.294, 19.864), (19.294, 20.000)]),
        ("M24", 24.0, 3.00, 21.00, [(20.752, 21.252), (20.752, 21.382), (20.752, 21.530)]),
        ("M27", 27.0, 3.00, 24.00, [(23.752, 24.252), (23.752, 24.382), (23.752, 24.530)]),
        ("M30", 30.0, 3.50, 26.50, [(26.211, 26.771), (26.211, 26.921), (26.211, 27.090)]),
        ("M36", 36.0, 4.00, 32.00, [(31.670, 32.270), (31.670, 32.420), (31.670, 32.610)]),
        ("M42", 42.0, 4.50, 37.50, [(37.129, 37.799), (37.129, 37.979), (37.129, 38.190)]),
        ("M48", 48.0, 5.00, 43.00, [(42.587, 43.297), (42.587, 43.497), (42.587, 43.720)])
    ]
    res = []
    for name, major, p, drill, tols in specs_data:
        res.append({
            "SpecName": name,
            "DisplayName": f"{name} (粗牙 P={p:.2f}mm)",
            "DiameterGroup": name,
            "PitchType": "粗牙",
            "MajorDiameter": major,
            "Pitch": p,
            "PitchDisplay": f"{p:.2f} mm",
            "DefaultTapDrill": drill,
            "Tolerances": [
                {"Grade": "6H", "PreferenceLevel": 1, "PreferenceDesc": "首选推荐", "MinorMin": tols[0][0], "MinorMax": tols[0][1], "RecommendedTapDrill": drill},
                {"Grade": "7H", "PreferenceLevel": 2, "PreferenceDesc": "次选", "MinorMin": tols[1][0], "MinorMax": tols[1][1], "RecommendedTapDrill": drill},
                {"Grade": "6G", "PreferenceLevel": 2, "PreferenceDesc": "次选(较大间隙)", "MinorMin": tols[1][0] + 0.02, "MinorMax": tols[1][1] + 0.02, "RecommendedTapDrill": drill + 0.1},
                {"Grade": "8H", "PreferenceLevel": 3, "PreferenceDesc": "不建议/迫不得已", "MinorMin": tols[2][0], "MinorMax": tols[2][1], "RecommendedTapDrill": drill}
            ]
        })
    return res

def get_metric_fine_specs():
    fines = [
        ("M6×0.75", "M6", 6.0, 0.75, 5.20, (5.188, 5.378)),
        ("M8×1.0", "M8", 8.0, 1.00, 7.00, (6.917, 7.153)),
        ("M8×0.75", "M8", 8.0, 0.75, 7.20, (7.188, 7.378)),
        ("M10×1.25", "M10", 10.0, 1.25, 8.80, (8.647, 8.912)),
        ("M10×1.0", "M10", 10.0, 1.00, 9.00, (8.917, 9.153)),
        ("M12×1.5", "M12", 12.0, 1.50, 10.50, (10.376, 10.676)),
        ("M12×1.25", "M12", 12.0, 1.25, 10.80, (10.647, 10.912)),
        ("M14×1.5", "M14", 14.0, 1.50, 12.50, (12.376, 12.676)),
        ("M16×1.5", "M16", 16.0, 1.50, 14.50, (14.376, 14.676)),
        ("M18×1.5", "M18", 18.0, 1.50, 16.50, (16.376, 16.676)),
        ("M20×1.5", "M20", 20.0, 1.50, 18.50, (18.376, 18.676)),
        ("M24×2.0", "M24", 24.0, 2.00, 22.00, (21.835, 22.210)),
        ("M24×1.5", "M24", 24.0, 1.50, 22.50, (22.376, 22.676)),
        ("M30×2.0", "M30", 30.0, 2.00, 28.00, (27.835, 28.210)),
        ("M30×1.5", "M30", 30.0, 1.50, 28.50, (28.376, 28.676))
    ]
    res = []
    for name, group, major, p, drill, tols in fines:
        res.append({
            "SpecName": name,
            "DisplayName": f"{name} (细牙 P={p:.2f}mm)",
            "DiameterGroup": group,
            "PitchType": "细牙",
            "MajorDiameter": major,
            "Pitch": p,
            "PitchDisplay": f"{p:.2f} mm",
            "DefaultTapDrill": drill,
            "Tolerances": [
                {"Grade": "6H", "PreferenceLevel": 1, "PreferenceDesc": "首选推荐", "MinorMin": tols[0], "MinorMax": tols[1], "RecommendedTapDrill": drill},
                {"Grade": "7H", "PreferenceLevel": 2, "PreferenceDesc": "次选", "MinorMin": tols[0], "MinorMax": tols[1] + 0.05, "RecommendedTapDrill": drill},
                {"Grade": "8H", "PreferenceLevel": 3, "PreferenceDesc": "不建议/迫不得已", "MinorMin": tols[0], "MinorMax": tols[1] + 0.10, "RecommendedTapDrill": drill}
            ]
        })
    return res

def get_pipe_g_specs():
    pipes = [
        ("G 1/8", 9.728, 0.907, 8.80, "28 TPI (0.907mm)", 8.566, 8.848),
        ("G 1/4", 13.157, 1.337, 11.80, "19 TPI (1.337mm)", 11.445, 11.890),
        ("G 3/8", 16.662, 1.337, 15.25, "19 TPI (1.337mm)", 14.950, 15.395),
        ("G 1/2", 20.955, 1.814, 19.00, "14 TPI (1.814mm)", 18.631, 19.172),
        ("G 3/4", 26.441, 1.814, 24.50, "14 TPI (1.814mm)", 24.117, 24.658),
        ("G 1", 33.249, 2.309, 30.75, "11 TPI (2.309mm)", 30.291, 30.931),
        ("G 1-1/4", 41.910, 2.309, 39.50, "11 TPI (2.309mm)", 38.952, 39.592),
        ("G 1-1/2", 47.803, 2.309, 45.25, "11 TPI (2.309mm)", 44.845, 45.485),
        ("G 2", 59.614, 2.309, 57.00, "11 TPI (2.309mm)", 56.656, 57.296)
    ]
    res = []
    for name, major, p, drill, p_disp, min_m, max_m in pipes:
        res.append({
            "SpecName": name,
            "DisplayName": f"{name} (55°圆柱管孔)",
            "DiameterGroup": name,
            "PitchType": "管螺纹",
            "MajorDiameter": major,
            "Pitch": p,
            "PitchDisplay": p_disp,
            "DefaultTapDrill": drill,
            "Tolerances": [
                {"Grade": "Class A", "PreferenceLevel": 1, "PreferenceDesc": "首选推荐", "MinorMin": min_m, "MinorMax": max_m, "RecommendedTapDrill": drill},
                {"Grade": "Class B", "PreferenceLevel": 2, "PreferenceDesc": "次选(放宽公差)", "MinorMin": min_m, "MinorMax": max_m + 0.08, "RecommendedTapDrill": drill}
            ]
        })
    return res

def get_pipe_rc_specs():
    rcs = [
        ("Rc 1/8", 9.728, 0.907, 8.60, "28 TPI (0.907mm)", 8.566, 8.848),
        ("Rc 1/4", 13.157, 1.337, 11.40, "19 TPI (1.337mm)", 11.445, 11.890),
        ("Rc 3/8", 16.662, 1.337, 15.00, "19 TPI (1.337mm)", 14.950, 15.395),
        ("Rc 1/2", 20.955, 1.814, 18.60, "14 TPI (1.814mm)", 18.631, 19.172),
        ("Rc 3/4", 26.441, 1.814, 24.10, "14 TPI (1.814mm)", 24.117, 24.658),
        ("Rc 1", 33.249, 2.309, 30.30, "11 TPI (2.309mm)", 30.291, 30.931),
        ("Rc 1-1/4", 41.910, 2.309, 39.00, "11 TPI (2.309mm)", 38.952, 39.592),
        ("Rc 1-1/2", 47.803, 2.309, 44.80, "11 TPI (2.309mm)", 44.845, 45.485),
        ("Rc 2", 59.614, 2.309, 56.70, "11 TPI (2.309mm)", 56.656, 57.296)
    ]
    res = []
    for name, major, p, drill, p_disp, min_m, max_m in rcs:
        res.append({
            "SpecName": name,
            "DisplayName": f"{name} (55°密封锥管孔)",
            "DiameterGroup": name,
            "PitchType": "密封管螺纹",
            "MajorDiameter": major,
            "Pitch": p,
            "PitchDisplay": p_disp,
            "DefaultTapDrill": drill,
            "Tolerances": [
                {"Grade": "标准公差", "PreferenceLevel": 1, "PreferenceDesc": "首选推荐", "MinorMin": min_m, "MinorMax": max_m, "RecommendedTapDrill": drill}
            ]
        })
    return res

def get_pipe_npt_specs():
    npts = [
        ("1/16-27 NPT", 7.894, 0.941, 6.15, "27 TPI"),
        ("1/8-27 NPT", 10.241, 0.941, 8.50, "27 TPI"),
        ("1/4-18 NPT", 13.614, 1.411, 11.10, "18 TPI"),
        ("3/8-18 NPT", 17.054, 1.411, 14.50, "18 TPI"),
        ("1/2-14 NPT", 21.222, 1.814, 17.80, "14 TPI"),
        ("3/4-14 NPT", 26.568, 1.814, 23.00, "14 TPI"),
        ("1-11.5 NPT", 33.228, 2.209, 29.00, "11.5 TPI"),
        ("1-1/4-11.5 NPT", 41.986, 2.209, 37.70, "11.5 TPI"),
        ("1-1/2-11.5 NPT", 48.057, 2.209, 44.00, "11.5 TPI"),
        ("2-11.5 NPT", 60.091, 2.209, 56.00, "11.5 TPI")
    ]
    res = []
    for name, major, p, drill, p_disp in npts:
        res.append({
            "SpecName": name,
            "DisplayName": f"{name} (60°美标锥管)",
            "DiameterGroup": name,
            "PitchType": "美标锥管螺纹",
            "MajorDiameter": major,
            "Pitch": p,
            "PitchDisplay": p_disp,
            "DefaultTapDrill": drill,
            "Tolerances": [
                {"Grade": "标准公差", "PreferenceLevel": 1, "PreferenceDesc": "首选推荐", "MinorMin": drill - 0.15, "MinorMax": drill + 0.20, "RecommendedTapDrill": drill}
            ]
        })
    return res

def get_trapezoidal_tr_specs():
    trs = [
        ("Tr 12×3", 12.0, 3.0, 9.50, 9.00, 9.75),
        ("Tr 14×3", 14.0, 3.0, 11.50, 11.00, 11.75),
        ("Tr 16×4", 16.0, 4.0, 12.50, 12.00, 13.00),
        ("Tr 18×4", 18.0, 4.0, 14.50, 14.00, 15.00),
        ("Tr 20×4", 20.0, 4.0, 16.50, 16.00, 17.00),
        ("Tr 22×5", 22.0, 5.0, 17.50, 17.00, 18.25),
        ("Tr 24×5", 24.0, 5.0, 19.50, 19.00, 20.25),
        ("Tr 28×5", 28.0, 5.0, 23.50, 23.00, 24.25),
        ("Tr 30×6", 30.0, 6.0, 24.50, 24.00, 25.50),
        ("Tr 32×6", 32.0, 6.0, 26.50, 26.00, 27.50),
        ("Tr 36×6", 36.0, 6.0, 30.50, 30.00, 31.50),
        ("Tr 40×7", 40.0, 7.0, 33.50, 33.00, 35.00),
        ("Tr 50×8", 50.0, 8.0, 42.50, 42.00, 44.00)
    ]
    res = []
    for name, major, p, drill, min_m, max_m in trs:
        res.append({
            "SpecName": name,
            "DisplayName": f"{name} (30°公制梯形)",
            "DiameterGroup": name.split('×')[0],
            "PitchType": "梯形螺纹",
            "MajorDiameter": major,
            "Pitch": p,
            "PitchDisplay": f"{p:.1f} mm",
            "DefaultTapDrill": drill,
            "Tolerances": [
                {"Grade": "7H", "PreferenceLevel": 1, "PreferenceDesc": "首选推荐", "MinorMin": min_m, "MinorMax": max_m, "RecommendedTapDrill": drill},
                {"Grade": "8H", "PreferenceLevel": 2, "PreferenceDesc": "次选", "MinorMin": min_m, "MinorMax": max_m + 0.15, "RecommendedTapDrill": drill},
                {"Grade": "9H", "PreferenceLevel": 3, "PreferenceDesc": "不建议/迫不得已", "MinorMin": min_m, "MinorMax": max_m + 0.35, "RecommendedTapDrill": drill}
            ]
        })
    return res

def get_sawtooth_specs():
    saws = [
        ("S 20×4", 20.0, 4.0, 16.0, 15.75, 16.35),
        ("S 24×5", 24.0, 5.0, 19.0, 18.70, 19.40),
        ("S 30×6", 30.0, 6.0, 24.0, 23.65, 24.45),
        ("S 36×6", 36.0, 6.0, 30.0, 29.65, 30.45),
        ("S 40×7", 40.0, 7.0, 33.0, 32.60, 33.50),
        ("S 50×8", 50.0, 8.0, 42.0, 41.55, 42.55)
    ]
    res = []
    for name, major, p, drill, min_m, max_m in saws:
        res.append({
            "SpecName": name,
            "DisplayName": f"{name} (锯齿形传动螺纹)",
            "DiameterGroup": name.split('×')[0],
            "PitchType": "锯齿形螺纹",
            "MajorDiameter": major,
            "Pitch": p,
            "PitchDisplay": f"{p:.1f} mm",
            "DefaultTapDrill": drill,
            "Tolerances": [
                {"Grade": "7H", "PreferenceLevel": 1, "PreferenceDesc": "首选推荐", "MinorMin": min_m, "MinorMax": max_m, "RecommendedTapDrill": drill},
                {"Grade": "8H", "PreferenceLevel": 2, "PreferenceDesc": "次选", "MinorMin": min_m, "MinorMax": max_m + 0.20, "RecommendedTapDrill": drill}
            ]
        })
    return res

def get_square_specs():
    sqs = [
        ("Sq 16×3", 16.0, 3.0, 13.0, 12.85, 13.20),
        ("Sq 20×4", 20.0, 4.0, 16.0, 15.80, 16.25),
        ("Sq 24×4", 24.0, 4.0, 20.0, 19.80, 20.25),
        ("Sq 30×5", 30.0, 5.0, 25.0, 24.75, 25.30),
        ("Sq 36×6", 36.0, 6.0, 30.0, 29.70, 30.35)
    ]
    res = []
    for name, major, p, drill, min_m, max_m in sqs:
        res.append({
            "SpecName": name,
            "DisplayName": f"{name} (矩形方螺纹)",
            "DiameterGroup": name.split('×')[0],
            "PitchType": "矩形螺纹",
            "MajorDiameter": major,
            "Pitch": p,
            "PitchDisplay": f"{p:.1f} mm",
            "DefaultTapDrill": drill,
            "Tolerances": [
                {"Grade": "基孔配合", "PreferenceLevel": 1, "PreferenceDesc": "首选推荐", "MinorMin": min_m, "MinorMax": max_m, "RecommendedTapDrill": drill}
            ]
        })
    return res

def get_unc_specs():
    uncs = [
        ("#4-40 UNC", 2.845, 0.635, 2.35, "40 TPI", 2.159, 2.388),
        ("#6-32 UNC", 3.505, 0.794, 2.85, "32 TPI", 2.642, 2.896),
        ("#8-32 UNC", 4.166, 0.794, 3.50, "32 TPI", 3.302, 3.531),
        ("#10-24 UNC", 4.826, 1.058, 3.90, "24 TPI", 3.683, 3.962),
        ("1/4-20 UNC", 6.350, 1.270, 5.10, "20 TPI", 4.978, 5.258),
        ("5/16-18 UNC", 7.938, 1.411, 6.60, "18 TPI", 6.401, 6.731),
        ("3/8-16 UNC", 9.525, 1.588, 8.00, "16 TPI", 7.798, 8.153),
        ("7/16-14 UNC", 11.112, 1.814, 9.40, "14 TPI", 9.144, 9.550),
        ("1/2-13 UNC", 12.700, 1.954, 10.80, "13 TPI", 10.592, 11.024),
        ("5/8-11 UNC", 15.875, 2.309, 13.50, "11 TPI", 13.386, 13.868),
        ("3/4-10 UNC", 19.050, 2.540, 16.50, "10 TPI", 16.307, 16.840),
        ("1-8 UNC", 25.400, 3.175, 22.25, "8 TPI", 21.971, 22.606)
    ]
    res = []
    for name, major, p, drill, p_disp, min_m, max_m in uncs:
        res.append({
            "SpecName": name,
            "DisplayName": f"{name} (美制粗牙)",
            "DiameterGroup": name.split('-')[0],
            "PitchType": "粗牙",
            "MajorDiameter": major,
            "Pitch": p,
            "PitchDisplay": p_disp,
            "DefaultTapDrill": drill,
            "Tolerances": [
                {"Grade": "2B", "PreferenceLevel": 1, "PreferenceDesc": "首选推荐(工业标准)", "MinorMin": min_m, "MinorMax": max_m, "RecommendedTapDrill": drill},
                {"Grade": "3B", "PreferenceLevel": 2, "PreferenceDesc": "次选(紧密配合)", "MinorMin": min_m, "MinorMax": max_m - 0.08, "RecommendedTapDrill": drill},
                {"Grade": "1B", "PreferenceLevel": 3, "PreferenceDesc": "不建议/宽松非标", "MinorMin": min_m - 0.1, "MinorMax": max_m + 0.12, "RecommendedTapDrill": drill}
            ]
        })
    return res

def get_unf_specs():
    unfs = [
        ("1/4-28 UNF", 6.350, 0.907, 5.50, "28 TPI", 5.359, 5.588),
        ("5/16-24 UNF", 7.938, 1.058, 6.90, "24 TPI", 6.782, 7.036),
        ("3/8-24 UNF", 9.525, 1.058, 8.50, "24 TPI", 8.382, 8.636),
        ("7/16-20 UNF", 11.112, 1.270, 9.90, "20 TPI", 9.728, 10.033),
        ("1/2-20 UNF", 12.700, 1.270, 11.50, "20 TPI", 11.328, 11.633)
    ]
    res = []
    for name, major, p, drill, p_disp, min_m, max_m in unfs:
        res.append({
            "SpecName": name,
            "DisplayName": f"{name} (美制细牙)",
            "DiameterGroup": name.split('-')[0],
            "PitchType": "细牙",
            "MajorDiameter": major,
            "Pitch": p,
            "PitchDisplay": p_disp,
            "DefaultTapDrill": drill,
            "Tolerances": [
                {"Grade": "2B", "PreferenceLevel": 1, "PreferenceDesc": "首选推荐", "MinorMin": min_m, "MinorMax": max_m, "RecommendedTapDrill": drill},
                {"Grade": "3B", "PreferenceLevel": 2, "PreferenceDesc": "次选(紧密配合)", "MinorMin": min_m, "MinorMax": max_m - 0.05, "RecommendedTapDrill": drill}
            ]
        })
    return res

def get_acme_specs():
    acmes = [
        ("1/2-10 ACME", 12.700, 2.540, 10.16, "10 TPI"),
        ("5/8-8 ACME", 15.875, 3.175, 12.70, "8 TPI"),
        ("3/4-6 ACME", 19.050, 4.233, 14.80, "6 TPI"),
        ("7/8-6 ACME", 22.225, 4.233, 18.00, "6 TPI"),
        ("1-5 ACME", 25.400, 5.080, 20.32, "5 TPI"),
        ("1-1/4-5 ACME", 31.750, 5.080, 26.67, "5 TPI")
    ]
    res = []
    for name, major, p, drill, p_disp in acmes:
        res.append({
            "SpecName": name,
            "DisplayName": f"{name} (29°美制梯形)",
            "DiameterGroup": name.split('-')[0],
            "PitchType": "梯形螺纹",
            "MajorDiameter": major,
            "Pitch": p,
            "PitchDisplay": p_disp,
            "DefaultTapDrill": drill,
            "Tolerances": [
                {"Grade": "2G", "PreferenceLevel": 1, "PreferenceDesc": "首选推荐(标准配合)", "MinorMin": drill - 0.15, "MinorMax": drill + 0.20, "RecommendedTapDrill": drill},
                {"Grade": "3G", "PreferenceLevel": 2, "PreferenceDesc": "次选(轻配合)", "MinorMin": drill - 0.20, "MinorMax": drill + 0.15, "RecommendedTapDrill": drill}
            ]
        })
    return res

# Build the 11 standards: ANSI Inch, ANSI Metric, AS, BSI, DIN, GB, IS, ISO, JIS, KS
standard_configs = [
    {
        "Id": "GB",
        "DisplayName": "GB (中国国家标准)",
        "Types": [
            ("普通螺纹", "MetricCoarse", "普通粗牙螺纹 (Metric Coarse - 60°)", "mm", get_metric_coarse_specs()),
            ("普通螺纹", "MetricFine", "普通细牙螺纹 (Metric Fine - 60°)", "mm", get_metric_fine_specs()),
            ("管螺纹", "PipeG", "55° 非密封圆柱管螺纹 (G / BSPP)", "mm", get_pipe_g_specs()),
            ("管螺纹", "PipeRc", "55° 密封圆锥管螺纹 (Rc / PT)", "mm", get_pipe_rc_specs()),
            ("管螺纹", "PipeNPT", "60° 锥管螺纹 (NPT)", "inch", get_pipe_npt_specs()),
            ("传动螺纹", "TrapezoidalTr", "30° 梯形螺纹 (Tr - 传动)", "mm", get_trapezoidal_tr_specs()),
            ("传动螺纹", "Sawtooth", "锯齿形螺纹 (S / Buttress)", "mm", get_sawtooth_specs()),
            ("传动螺纹", "Square", "矩形螺纹 (Square Thread)", "mm", get_square_specs())
        ]
    },
    {
        "Id": "ISO",
        "DisplayName": "ISO (国际标准化组织)",
        "Types": [
            ("普通螺纹", "ISOMetricCoarse", "ISO 公制粗牙 (ISO 261 / ISO 965)", "mm", get_metric_coarse_specs()),
            ("普通螺纹", "ISOMetricFine", "ISO 公制细牙 (ISO 261 / ISO 965)", "mm", get_metric_fine_specs()),
            ("管螺纹", "ISOPipeG", "ISO 228-1 圆柱管螺纹 (G)", "mm", get_pipe_g_specs()),
            ("管螺纹", "ISOPipeRc", "ISO 7-1 密封管螺纹 (Rc / Rp / R)", "mm", get_pipe_rc_specs()),
            ("传动螺纹", "ISOTrapezoidalTr", "ISO 2901 梯形螺纹 (Tr)", "mm", get_trapezoidal_tr_specs())
        ]
    },
    {
        "Id": "DIN",
        "DisplayName": "DIN (德国工业标准)",
        "Types": [
            ("普通螺纹", "DINMetricCoarse", "DIN 13-1 公制粗牙", "mm", get_metric_coarse_specs()),
            ("普通螺纹", "DINMetricFine", "DIN 13-2~11 公制细牙", "mm", get_metric_fine_specs()),
            ("管螺纹", "DINPipeG", "DIN EN ISO 228 管螺纹 (G)", "mm", get_pipe_g_specs()),
            ("管螺纹", "DINPipeRc", "DIN EN 10226 密封锥管 (Rc/R)", "mm", get_pipe_rc_specs()),
            ("传动螺纹", "DINTrapezoidalTr", "DIN 103 梯形螺纹 (Tr)", "mm", get_trapezoidal_tr_specs()),
            ("传动螺纹", "DINSawtooth", "DIN 513 锯齿形螺纹 (S)", "mm", get_sawtooth_specs())
        ]
    },
    {
        "Id": "ANSI Metric",
        "DisplayName": "ANSI Metric (美标公制 M 系列)",
        "Types": [
            ("普通螺纹", "ANSIMetricCoarse", "ASME B1.13M 公制粗牙", "mm", get_metric_coarse_specs()),
            ("普通螺纹", "ANSIMetricFine", "ASME B1.13M 公制细牙", "mm", get_metric_fine_specs()),
            ("传动螺纹", "ANSIMetricTr", "ASME B1.5M 公制梯形螺纹", "mm", get_trapezoidal_tr_specs())
        ]
    },
    {
        "Id": "ANSI Inch",
        "DisplayName": "ANSI Inch (美标英制统一螺纹)",
        "Types": [
            ("普通螺纹", "UNC", "美制粗牙统一螺纹 (UNC - 60°)", "inch", get_unc_specs()),
            ("普通螺纹", "UNF", "美制细牙统一螺纹 (UNF - 60°)", "inch", get_unf_specs()),
            ("管螺纹", "NPT", "美制锥管螺纹 (NPT - 60°)", "inch", get_pipe_npt_specs()),
            ("传动螺纹", "ACME", "29° 爱克姆梯形螺纹 (ACME)", "inch", get_acme_specs())
        ]
    },
    {
        "Id": "BSI",
        "DisplayName": "BSI (英国标准学会 BS)",
        "Types": [
            ("普通螺纹", "BSMetricCoarse", "BS 3643 公制粗牙", "mm", get_metric_coarse_specs()),
            ("普通螺纹", "BSMetricFine", "BS 3643 公制细牙", "mm", get_metric_fine_specs()),
            ("普通螺纹", "BSW", "BS 84 惠氏粗牙螺纹 (BSW/UNC)", "inch", get_unc_specs()),
            ("管螺纹", "BSPP", "BS 2779 圆柱管螺纹 (G / BSPP)", "mm", get_pipe_g_specs()),
            ("管螺纹", "BSPT", "BS 21 锥管螺纹 (Rc / BSPT)", "mm", get_pipe_rc_specs()),
            ("传动螺纹", "BSTrapezoidalTr", "BS 5346 梯形螺纹 (Tr)", "mm", get_trapezoidal_tr_specs())
        ]
    },
    {
        "Id": "JIS",
        "DisplayName": "JIS (日本工业标准)",
        "Types": [
            ("普通螺纹", "JISMetricCoarse", "JIS B 0205 公制粗牙", "mm", get_metric_coarse_specs()),
            ("普通螺纹", "JISMetricFine", "JIS B 0207 公制细牙", "mm", get_metric_fine_specs()),
            ("管螺纹", "JISPipeG", "JIS B 0202 平行管螺纹 (G / PF)", "mm", get_pipe_g_specs()),
            ("管螺纹", "JISPipeRc", "JIS B 0203 锥形管螺纹 (Rc / PT)", "mm", get_pipe_rc_specs()),
            ("传动螺纹", "JISTrapezoidalTr", "JIS B 0216 梯形螺纹 (Tr)", "mm", get_trapezoidal_tr_specs())
        ]
    },
    {
        "Id": "KS",
        "DisplayName": "KS (韩国工业标准)",
        "Types": [
            ("普通螺纹", "KSMetricCoarse", "KS B 0201 公制粗牙", "mm", get_metric_coarse_specs()),
            ("普通螺纹", "KSMetricFine", "KS B 0204 公制细牙", "mm", get_metric_fine_specs()),
            ("管螺纹", "KSPipeG", "KS B 0221 管螺纹 (G / PF)", "mm", get_pipe_g_specs()),
            ("管螺纹", "KSPipeRc", "KS B 0222 锥管螺纹 (Rc / PT)", "mm", get_pipe_rc_specs()),
            ("传动螺纹", "KSTrapezoidalTr", "KS B 0229 梯形螺纹 (Tr)", "mm", get_trapezoidal_tr_specs())
        ]
    },
    {
        "Id": "AS",
        "DisplayName": "AS (澳大利亚标准)",
        "Types": [
            ("普通螺纹", "ASMetricCoarse", "AS 1275 公制粗牙", "mm", get_metric_coarse_specs()),
            ("普通螺纹", "ASMetricFine", "AS 1275 公制细牙", "mm", get_metric_fine_specs()),
            ("普通螺纹", "ASUNC", "AS 1110 统一粗牙 (UNC)", "inch", get_unc_specs()),
            ("管螺纹", "ASPipeG", "AS 1722.2 管螺纹 (G)", "mm", get_pipe_g_specs()),
            ("管螺纹", "ASPipeRc", "AS 1722.1 密封锥管 (Rc)", "mm", get_pipe_rc_specs()),
            ("传动螺纹", "ASTrapezoidalTr", "AS 4381 梯形螺纹 (Tr)", "mm", get_trapezoidal_tr_specs())
        ]
    },
    {
        "Id": "IS",
        "DisplayName": "IS (印度工业标准)",
        "Types": [
            ("普通螺纹", "ISMetricCoarse", "IS 4218 公制粗牙", "mm", get_metric_coarse_specs()),
            ("普通螺纹", "ISMetricFine", "IS 4218 公制细牙", "mm", get_metric_fine_specs()),
            ("管螺纹", "ISPipeG", "IS 2643 管螺纹 (G)", "mm", get_pipe_g_specs()),
            ("管螺纹", "ISPipeRc", "IS 554 锥管螺纹 (Rc)", "mm", get_pipe_rc_specs()),
            ("传动螺纹", "ISTrapezoidalTr", "IS 7008 梯形螺纹 (Tr)", "mm", get_trapezoidal_tr_specs())
        ]
    }
]

db = {"Standards": []}
for cfg in standard_configs:
    std_item = {
        "Id": cfg["Id"],
        "DisplayName": cfg["DisplayName"],
        "ThreadTypes": []
    }
    for cat, tid, tname, unit, specs in cfg["Types"]:
        std_item["ThreadTypes"].append({
            "Id": tid,
            "Category": cat,
            "DisplayName": tname,
            "Unit": unit,
            "Specifications": specs
        })
    db["Standards"].append(std_item)

output_path = r"c:\DownLoad\Application_Design\SW_putin_design\ThreadTapAssistant\Data\ThreadStandards.json"
with open(output_path, "w", encoding="utf-8") as f:
    json.dump(db, f, ensure_ascii=False, indent=2)

print("ThreadStandards.json successfully generated with 11 global standards!")
